using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

public class ImprovementPlan
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    
    // الهدف أو الجدارة التي تسببت في خطة التحسين
    public string SourceItem { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // "هدف وظيفي" أو "جدارة"
    
    public double CurrentScore { get; set; }
    public double TargetScore { get; set; }
    
    // المهام التنفيذية
    public List<ImprovementTask> ExecutiveTasks { get; set; } = new();
    
    public string Timeframe { get; set; } = "3 أشهر";
    public string Priority { get; set; } = "عالية"; 
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;
    
    public int ProgressPercentage => ExecutiveTasks.Count == 0 ? 0 : 
        (int)(ExecutiveTasks.Count(t => t.Status == "مكتمل") * 100.0 / ExecutiveTasks.Count + 
              ExecutiveTasks.Count(t => t.Status == "قيد التنفيذ") * 50.0 / ExecutiveTasks.Count);
}

public class ImprovementTask
{
    public int Id { get; set; }
    public int ImprovementPlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = "الموظف والمدير";
    public string Deadline { get; set; } = "نهاية الشهر";
    public string Status { get; set; } = "مجدول"; // مجدول، قيد التنفيذ، مكتمل
    public string? EmployeeNote { get; set; }
}
