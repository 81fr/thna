using HRPerformanceSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace HRPerformanceSystem.Services;

public class SearchResult
{
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Link { get; set; } = "";
    public string Icon { get; set; } = "search";
}

public class SearchService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public SearchService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<SearchResult>> SearchAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2) return new();

        var results = new List<SearchResult>();
        using var db = _factory.CreateDbContext();

        // 1. Search Employees
        var employees = await db.Employees
            .Where(e => e.FullName.Contains(query) || e.EmployeeNumber.Contains(query))
            .Take(5)
            .ToListAsync();

        foreach (var e in employees)
        {
            results.Add(new SearchResult 
            { 
                Title = e.FullName, 
                Subtitle = $"موظف • {e.Department}", 
                Link = $"employees/view/{e.Id}",
                Icon = "user"
            });
        }

        // 2. Search Pages (Static)
        var pages = new[] {
            new { T = "لوحة القيادة المؤسسية", L = "institutional-dashboard", Q = "داشبورد لوحة قيادة" },
            new { T = "مواثيق الأداء", L = "evaluations", Q = "مواثيق تقييم" },
            new { T = "رؤى الذكاء الاصطناعي", L = "ai-insights", Q = "ذكاء اصطناعي AI" },
            new { T = "إعدادات الحساب", L = "Account/Manage", Q = "حسابي كلمة مرور" }
        };

        foreach (var p in pages)
        {
            if (p.T.Contains(query) || p.Q.Contains(query))
            {
                results.Add(new SearchResult { Title = p.T, Subtitle = "صفحة في النظام", Link = p.L, Icon = "file-text" });
            }
        }

        return results;
    }
}
