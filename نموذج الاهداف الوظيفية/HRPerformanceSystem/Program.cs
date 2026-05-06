using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HRPerformanceSystem.Components;
using HRPerformanceSystem.Components.Account;
using HRPerformanceSystem.Data;
using HRPerformanceSystem.Models;
using HRPerformanceSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddAuthorization();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register DbContext AND DbContextFactory for safe use in InteractiveServer
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString), ServiceLifetime.Scoped);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 3;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredUniqueChars = 0;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Register custom services
builder.Services.AddScoped<CalculationService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddScoped<AIInsightsService>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddScoped<SearchService>();

var app = builder.Build();

// Auto-create/migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    // Identity Role & User Seed
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string[] requiredRoles = { "Admin", "HR", "Manager", "Employee" };
    foreach (var r in requiredRoles)
    {
        if (!roleManager.RoleExistsAsync(r).Result)
        {
            roleManager.CreateAsync(new IdentityRole(r)).Wait();
        }
    }

    if (userManager.FindByNameAsync("81fr").Result == null)
    {
        var adminUser = new ApplicationUser { UserName = "81fr", Email = "admin@admin.com", EmailConfirmed = true };
        if (userManager.CreateAsync(adminUser, "123").Result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
        }
    }

    // Seed users for main employees
    var seedEmployees = new[] 
    { 
        new { Name = "أحمد محمد العمري", Number = "EMP001", Role = "HR" },
        new { Name = "فاطمة عبدالله الشمري", Number = "EMP002", Role = "HR" },
        new { Name = "محمد سعد الدوسري", Number = "EMP003", Role = "Employee" }
    };

    foreach (var se in seedEmployees)
    {
        string username = se.Number.ToLower();
        if (userManager.FindByNameAsync(username).Result == null)
        {
            var user = new ApplicationUser { UserName = username, Email = $"{username}@traof.org", EmailConfirmed = true };
            if (userManager.CreateAsync(user, "123").Result.Succeeded)
            {
                userManager.AddToRoleAsync(user, se.Role).Wait();
                var emp = db.Employees.FirstOrDefault(e => e.EmployeeNumber == se.Number);
                if (emp != null)
                {
                    emp.UserId = user.Id;
                }
            }
        }
    }
    db.SaveChanges();

    // Seed Trial Evaluation if empty
    if (db.Evaluations.Count() <= 1)
    {
        // 1. Fatima (Previous Seed) - Ensure it exists
        var fatima = db.Employees.FirstOrDefault(e => e.EmployeeNumber == "EMP002");
        
        // 2. Mohammed Otibi (New Transfer Case)
        var motibi = db.Employees.FirstOrDefault(e => e.EmployeeNumber == "EMP004");
        if (motibi == null)
        {
            motibi = new Employee { FullName = "محمد العتيبي", EmployeeNumber = "EMP004", JobTitle = "مهندس صيانة/مشروعات", Department = "المشروعات", DirectManager = "سلمان الفهد", AttendanceType = AttendanceType.FullTime, CharterType = CharterType.Employee, CreatedAt = DateTime.Now };
            db.Employees.Add(motibi);
            db.SaveChanges();
        }

        if (!db.Evaluations.Any(e => e.EmployeeId == motibi.Id))
        {
            var transferEval = new PerformanceEvaluation
            {
                EmployeeId = motibi.Id,
                Year = 2026,
                ReviewerName = "سلمان الفهد",
                Status = EvaluationStatus.Approved,
                CreatedAt = DateTime.Now,
                Notes = "حالة انتقال: شهري 1-2 (التشغيل) وشهر 3 (المشروعات)"
            };
            db.Evaluations.Add(transferEval);
            db.SaveChanges();

            // Dept A Goals (Months 1-2)
            db.Goals.AddRange(
                new ObjectiveGoal { 
                    EvaluationId = transferEval.Id, GoalDescription = "[إدارة التشغيل] صيانة المرافق", Weight = 30, DisplayOrder = 1,
                    Q1M1Target = 10, Q1M2Target = 10, Q1M3Target = 0, // نشط في 1 و 2
                    Q1M1Actual = 10, Q1M2Actual = 9 
                },
                new ObjectiveGoal { 
                    EvaluationId = transferEval.Id, GoalDescription = "[إدارة التشغيل] توفير قطع الغيار", Weight = 35, DisplayOrder = 2,
                    Q1M1Target = 5, Q1M2Target = 5, Q1M3Target = 0, // نشط في 1 و 2
                    Q1M1Actual = 5, Q1M2Actual = 5 
                }
            );

            // Dept B Goals (Month 3 onwards)
            db.Goals.AddRange(
                new ObjectiveGoal { 
                    EvaluationId = transferEval.Id, GoalDescription = "[إدارة المشروعات] الإشراف على الموقع الجديد", Weight = 65, DisplayOrder = 3,
                    Q1M1Target = 0, Q1M2Target = 0, Q1M3Target = 1, // يبدأ من شهر 3
                    Q1M3Actual = 1 
                }
            );

            // Default Competencies
            foreach(var name in new[]{"الالتزام بالحضور","التطوع","التعلم","المشاركة","التمكن التقني","الذكاء التواصلي","التعاون","الإبداع","البونص"})
            {
                db.Competencies.Add(new CompetencyEvaluation { 
                    EvaluationId = transferEval.Id, Name = name, Type = name.Contains("بونص") ? CompetencyType.Bonus : CompetencyType.Core, 
                    Weight = name.Contains("بونص") ? 5 : 4, Q1M1Rating = 3, Q1M2Rating = 3, Q1M3Rating = 3 
                });
            }

            db.SaveChanges();
        }

        // Seed initial notification
        if (!db.Notifications.Any())
        {
            db.Notifications.Add(new Notification
            {
                Title = "مرحباً بك في نظام إدارة الأداء",
                Message = "تم تفعيل النظام بنجاح. ابدأ بإنشاء مواثيق أداء لموظفيك.",
                Type = NotificationType.Info,
                Link = "/evaluations/new"
            });
            db.SaveChanges();
        }

        // Call the large data seeder
        DbSeeder.SeedLargeData(db);
    }

    // Ensure all employees have users
    var allEmployees = db.Employees.Where(e => string.IsNullOrEmpty(e.UserId)).ToList();
    foreach (var emp in allEmployees)
    {
        string username = emp.EmployeeNumber.ToLower();
        if (userManager.FindByNameAsync(username).Result == null)
        {
            var user = new ApplicationUser { UserName = username, Email = $"{username}@traof.org", EmailConfirmed = true };
            if (userManager.CreateAsync(user, "123").Result.Succeeded)
            {
                userManager.AddToRoleAsync(user, emp.CharterType == CharterType.Leader ? "Manager" : "Employee").Wait();
                emp.UserId = user.Id;
            }
        }
    }
    db.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Excel export endpoint
app.MapGet("/api/export/employee/{evalId:int}", async (int evalId, ExcelExportService exportService) =>
{
    try
    {
        var bytes = await exportService.ExportEmployeeReportAsync(evalId);
        return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"تقييم_أداء_{evalId}.xlsx");
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/api/export/all", async (ExcelExportService exportService) =>
{
    var bytes = await exportService.ExportAllEmployeesReportAsync();
    return Results.File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"ملخص_أداء_{DateTime.Now:yyyyMMdd}.xlsx");
});

app.Run();
