using System;
using System.ComponentModel.DataAnnotations;

namespace HRPerformanceSystem.Models;

public class DashboardMetric
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Module { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public double Value { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.Now;

    [MaxLength(100)]
    public string? UpdatedBy { get; set; }
}
