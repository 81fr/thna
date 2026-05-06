using HRPerformanceSystem.Data;
using HRPerformanceSystem.Models;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace HRPerformanceSystem.Services;

/// <summary>
/// خدمة تصدير البيانات لصيغة Excel
/// </summary>
public class ExcelExportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;
    private readonly CalculationService _calcService;

    public ExcelExportService(IDbContextFactory<ApplicationDbContext> factory, CalculationService calcService)
    {
        _factory = factory;
        _calcService = calcService;
    }

    /// <summary>
    /// تصدير تقرير أداء موظف بصيغة Excel
    /// </summary>
    public async Task<byte[]> ExportEmployeeReportAsync(int evaluationId)
    {
        using var db = _factory.CreateDbContext();
        var eval = await db.Evaluations
            .Include(e => e.Employee)
            .Include(e => e.Goals)
            .Include(e => e.Competencies)
            .Include(e => e.AttendanceRecords)
            .FirstOrDefaultAsync(e => e.Id == evaluationId);

        if (eval == null) throw new InvalidOperationException("التقييم غير موجود");

        ExcelPackage.License.SetNonCommercialOrganization("TRAOF");
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add($"تقييم {eval.Employee.FullName}");

        ws.View.RightToLeft = true;

        // Header
        int row = 1;
        ws.Cells[row, 1].Value = "ميثاق الأداء الوظيفي";
        ws.Cells[row, 1].Style.Font.Size = 18;
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 1, row, 6].Merge = true;
        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

        // Employee Info
        row += 2;
        SetInfoRow(ws, row, "الموظف", eval.Employee.FullName); row++;
        SetInfoRow(ws, row, "الرقم الوظيفي", eval.Employee.EmployeeNumber); row++;
        SetInfoRow(ws, row, "الإدارة", eval.Employee.Department); row++;
        SetInfoRow(ws, row, "المسمى الوظيفي", eval.Employee.JobTitle); row++;
        SetInfoRow(ws, row, "السنة", eval.Year.ToString()); row++;
        SetInfoRow(ws, row, "التقييم النهائي", $"{eval.FinalScore:F1}% - {_calcService.GetRating(eval.FinalScore)}"); row++;

        // Goals Section
        row += 2;
        ws.Cells[row, 1].Value = "الأهداف الوظيفية (65%)";
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 1].Style.Font.Size = 14;
        ws.Cells[row, 1, row, 6].Merge = true;
        StyleHeader(ws.Cells[row, 1, row, 6]);
        row++;

        // Goals Table Header
        var headers = new[] { "#", "الهدف", "الوزن %", "الربع 1", "الربع 2", "الربع 3", "الربع 4" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[row, i + 1].Value = headers[i];
            ws.Cells[row, i + 1].Style.Font.Bold = true;
            ws.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 245, 249));
        }
        row++;

        int goalIdx = 1;
        foreach (var g in eval.Goals.OrderBy(g => g.DisplayOrder))
        {
            ws.Cells[row, 1].Value = goalIdx++;
            ws.Cells[row, 2].Value = g.GoalDescription;
            ws.Cells[row, 3].Value = g.Weight;
            ws.Cells[row, 4].Value = Math.Round(g.CalcQ1Score(), 2);
            ws.Cells[row, 5].Value = Math.Round(g.CalcQ2Score(), 2);
            ws.Cells[row, 6].Value = Math.Round(g.CalcQ3Score(), 2);
            ws.Cells[row, 7].Value = Math.Round(g.CalcQ4Score(), 2);
            row++;
        }

        // Competencies Section
        row += 2;
        ws.Cells[row, 1].Value = "الجدارات (35%)";
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 1].Style.Font.Size = 14;
        ws.Cells[row, 1, row, 6].Merge = true;
        StyleHeader(ws.Cells[row, 1, row, 6]);
        row++;

        var compHeaders = new[] { "#", "الجدارة", "الوزن", "الربع 1", "الربع 2", "الربع 3", "الربع 4" };
        for (int i = 0; i < compHeaders.Length; i++)
        {
            ws.Cells[row, i + 1].Value = compHeaders[i];
            ws.Cells[row, i + 1].Style.Font.Bold = true;
            ws.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 245, 249));
        }
        row++;

        int compIdx = 1;
        foreach (var c in eval.Competencies.OrderBy(c => c.DisplayOrder))
        {
            ws.Cells[row, 1].Value = compIdx++;
            ws.Cells[row, 2].Value = c.Name;
            ws.Cells[row, 3].Value = c.Weight;
            ws.Cells[row, 4].Value = Math.Round(c.CalcQ1Score(), 2);
            ws.Cells[row, 5].Value = Math.Round(c.CalcQ2Score(), 2);
            ws.Cells[row, 6].Value = Math.Round(c.CalcQ3Score(), 2);
            ws.Cells[row, 7].Value = Math.Round(c.CalcQ4Score(), 2);
            row++;
        }

        // Quarterly Summary
        row += 2;
        ws.Cells[row, 1].Value = "ملخص الأرباع";
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 1].Style.Font.Size = 14;
        ws.Cells[row, 1, row, 6].Merge = true;
        StyleHeader(ws.Cells[row, 1, row, 6]);
        row++;

        for (int q = 1; q <= 4; q++)
        {
            var qScore = _calcService.CalculateQuarterTotal(eval, q);
            ws.Cells[row, 1].Value = $"الربع {q}";
            ws.Cells[row, 2].Value = $"{qScore:F1}%";
            ws.Cells[row, 3].Value = _calcService.GetRating(qScore);
            row++;
        }

        ws.Cells[row, 1].Value = "المعدل السنوي";
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 2].Value = $"{eval.FinalScore:F1}%";
        ws.Cells[row, 2].Style.Font.Bold = true;
        ws.Cells[row, 3].Value = _calcService.GetRating(eval.FinalScore);
        ws.Cells[row, 3].Style.Font.Bold = true;

        // Auto-fit columns
        ws.Cells.AutoFitColumns();
        ws.Column(2).Width = 40;

        return package.GetAsByteArray();
    }

    /// <summary>
    /// تصدير ملخص أداء جميع الموظفين
    /// </summary>
    public async Task<byte[]> ExportAllEmployeesReportAsync()
    {
        using var db = _factory.CreateDbContext();
        var evals = await db.Evaluations
            .Include(e => e.Employee)
            .Include(e => e.Goals)
            .Include(e => e.Competencies)
            .Include(e => e.AttendanceRecords)
            .ToListAsync();

        ExcelPackage.License.SetNonCommercialOrganization("TRAOF");
        using var package = new ExcelPackage();
        var ws = package.Workbook.Worksheets.Add("ملخص الأداء العام");
        ws.View.RightToLeft = true;

        int row = 1;
        ws.Cells[row, 1].Value = $"ملخص أداء الموظفين - {DateTime.Now.Year}";
        ws.Cells[row, 1].Style.Font.Size = 16;
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 1, row, 9].Merge = true;
        ws.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        row += 2;

        var headers = new[] { "#", "الموظف", "الرقم الوظيفي", "الإدارة", "الربع 1", "الربع 2", "الربع 3", "الربع 4", "التقييم النهائي", "المستوى" };
        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cells[row, i + 1].Value = headers[i];
            ws.Cells[row, i + 1].Style.Font.Bold = true;
            ws.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 165, 155));
            ws.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
        }
        row++;

        int idx = 1;
        foreach (var eval in evals.OrderByDescending(e => e.FinalScore))
        {
            ws.Cells[row, 1].Value = idx++;
            ws.Cells[row, 2].Value = eval.Employee.FullName;
            ws.Cells[row, 3].Value = eval.Employee.EmployeeNumber;
            ws.Cells[row, 4].Value = eval.Employee.Department;
            ws.Cells[row, 5].Value = eval.Q1Score > 0 ? $"{eval.Q1Score:F1}%" : "—";
            ws.Cells[row, 6].Value = eval.Q2Score > 0 ? $"{eval.Q2Score:F1}%" : "—";
            ws.Cells[row, 7].Value = eval.Q3Score > 0 ? $"{eval.Q3Score:F1}%" : "—";
            ws.Cells[row, 8].Value = eval.Q4Score > 0 ? $"{eval.Q4Score:F1}%" : "—";
            ws.Cells[row, 9].Value = $"{eval.FinalScore:F1}%";
            ws.Cells[row, 9].Style.Font.Bold = true;
            ws.Cells[row, 10].Value = _calcService.GetRating(eval.FinalScore);

            // Alternating row colors
            if (idx % 2 == 0)
            {
                for (int c = 1; c <= 10; c++)
                {
                    ws.Cells[row, c].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    ws.Cells[row, c].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(248, 250, 252));
                }
            }
            row++;
        }

        ws.Cells.AutoFitColumns();
        return package.GetAsByteArray();
    }

    private void SetInfoRow(ExcelWorksheet ws, int row, string label, string value)
    {
        ws.Cells[row, 1].Value = label;
        ws.Cells[row, 1].Style.Font.Bold = true;
        ws.Cells[row, 2].Value = value;
        ws.Cells[row, 2, row, 4].Merge = true;
    }

    private void StyleHeader(ExcelRange range)
    {
        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
        range.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(0, 165, 155));
        range.Style.Font.Color.SetColor(Color.White);
    }
}
