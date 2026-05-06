using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using HRPerformanceSystem.Models;

namespace HRPerformanceSystem.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PerformanceEvaluation> Evaluations => Set<PerformanceEvaluation>();
    public DbSet<ObjectiveGoal> Goals => Set<ObjectiveGoal>();
    public DbSet<CompetencyEvaluation> Competencies => Set<CompetencyEvaluation>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<SurveyResult> SurveyResults => Set<SurveyResult>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<ImprovementPlan> ImprovementPlans => Set<ImprovementPlan>();
    public DbSet<ImprovementTask> ImprovementTasks => Set<ImprovementTask>();
    public DbSet<EvaluationComment> EvaluationComments => Set<EvaluationComment>();
    public DbSet<DashboardMetric> DashboardMetrics => Set<DashboardMetric>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Employee>(e =>
        {
            e.HasIndex(x => x.EmployeeNumber).IsUnique();
            e.Property(x => x.FullName).HasMaxLength(200);
            e.Property(x => x.Department).HasMaxLength(200);
            e.Property(x => x.JobTitle).HasMaxLength(200);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Phone).HasMaxLength(20);
            e.HasIndex(x => x.Department);
            e.HasIndex(x => x.IsActive);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<PerformanceEvaluation>(e =>
        {
            e.HasOne(x => x.Employee)
                .WithMany(x => x.Evaluations)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.EmployeeId, x.Year });
            e.HasIndex(x => x.Status);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        builder.Entity<ObjectiveGoal>(e =>
        {
            e.HasOne(x => x.Evaluation)
                .WithMany(x => x.Goals)
                .HasForeignKey(x => x.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<CompetencyEvaluation>(e =>
        {
            e.HasOne(x => x.Evaluation)
                .WithMany(x => x.Competencies)
                .HasForeignKey(x => x.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AttendanceRecord>(e =>
        {
            e.HasOne(x => x.Evaluation)
                .WithMany(x => x.AttendanceRecords)
                .HasForeignKey(x => x.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.EvaluationId, x.Month });
        });

        builder.Entity<SurveyResult>(e =>
        {
            e.HasOne(x => x.Evaluation)
                .WithMany(x => x.SurveyResults)
                .HasForeignKey(x => x.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<AuditLog>(e =>
        {
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => new { x.EntityType, x.EntityId });
            e.Property(x => x.Description).HasMaxLength(500);
        });

        builder.Entity<Notification>(e =>
        {
            e.HasIndex(x => x.IsRead);
            e.HasIndex(x => x.CreatedAt);
            e.Property(x => x.Title).HasMaxLength(200);
            e.Property(x => x.Message).HasMaxLength(500);
            e.HasIndex(x => x.TargetUserId);
        });

        builder.Entity<ImprovementPlan>(e =>
        {
            e.HasMany(x => x.ExecutiveTasks)
                .WithOne()
                .HasForeignKey(x => x.ImprovementPlanId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.EmployeeId);
            e.HasIndex(x => x.UserId);
        });

        // Seed default data
        SeedData(builder);
    }

    private void SeedData(ModelBuilder builder)
    {
        var fixedDate = new DateTime(2026, 1, 1);
        builder.Entity<Employee>().HasData(
            new Employee { Id = 1, FullName = "أحمد محمد العمري", EmployeeNumber = "EMP001", JobTitle = "مدير إدارة", Department = "الموارد البشرية", DirectManager = "خالد السعيد", AttendanceType = AttendanceType.FullTime, CharterType = CharterType.Leader, CreatedAt = fixedDate },
            new Employee { Id = 2, FullName = "فاطمة عبدالله الشمري", EmployeeNumber = "EMP002", JobTitle = "أخصائي موارد بشرية", Department = "الموارد البشرية", DirectManager = "أحمد محمد العمري", AttendanceType = AttendanceType.FullTime, CharterType = CharterType.Employee, CreatedAt = fixedDate },
            new Employee { Id = 3, FullName = "محمد سعد الدوسري", EmployeeNumber = "EMP003", JobTitle = "محلل بيانات", Department = "تقنية المعلومات", DirectManager = "سارة الحربي", AttendanceType = AttendanceType.FullTime, CharterType = CharterType.Employee, CreatedAt = fixedDate }
        );
    }
}
