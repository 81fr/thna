using HRPerformanceSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HRPerformanceSystem.Data;

public static class DbSeeder
{
    public static void SeedLargeData(ApplicationDbContext db)
    {
        if (db.Employees.Count() > 10) return; // Already seeded

        var departments = new[] { "الموارد البشرية", "تقنية المعلومات", "المالية", "المشاريع", "الخدمات المساندة", "العلاقات العامة", "الجودة", "التشغيل" };
        var titles = new[] { "مدير قسم", "أخصائي أول", "مهندس", "محاسب", "سكرتير تنفيذي", "مشرف ميداني", "محلل بيانات", "منسق إداري" };
        var names = new[] { "أحمد", "سارة", "خالد", "نورة", "عبدالله", "ليلى", "فهد", "ريم", "محمد", "منى", "سلطان", "هيا", "بدر", "أمل", "فيصل", "جواهر" };
        var families = new[] { "العتيبي", "القحطاني", "الشمري", "الدوسري", "المالكي", "الغامدي", "الزهراني", "الحربي", "المطيري", "السبيعي" };

        var random = new Random();
        var employees = new List<Employee>();

        for (int i = 1; i <= 50; i++)
        {
            var emp = new Employee
            {
                FullName = $"{names[random.Next(names.Length)]} {names[random.Next(names.Length)]} {families[random.Next(families.Length)]}",
                EmployeeNumber = $"EMP{100 + i}",
                Department = departments[random.Next(departments.Length)],
                JobTitle = titles[random.Next(titles.Length)],
                DirectManager = names[random.Next(names.Length)] + " " + families[random.Next(families.Length)],
                AttendanceType = random.Next(2) == 0 ? AttendanceType.FullTime : AttendanceType.PartTime,
                CharterType = random.Next(5) == 0 ? CharterType.Leader : CharterType.Employee,
                IsActive = true,
                CreatedAt = DateTime.Now.AddDays(-random.Next(100, 500))
            };
            employees.Add(emp);
        }

        db.Employees.AddRange(employees);
        db.SaveChanges();

        var evaluations = new List<PerformanceEvaluation>();
        foreach (var emp in employees)
        {
            // Create evaluations for 2025 and 2026
            for (int year = 2025; year <= 2026; year++)
            {
                var eval = new PerformanceEvaluation
                {
                    EmployeeId = emp.Id,
                    Year = year,
                    ReviewerName = emp.DirectManager,
                    Status = random.Next(3) switch { 0 => EvaluationStatus.Approved, 1 => EvaluationStatus.InReview, _ => EvaluationStatus.Draft },
                    CreatedAt = DateTime.Now.AddMonths(-random.Next(1, 12)),
                    UpdatedAt = DateTime.Now,
                    Q1Score = random.Next(70, 100),
                    Q2Score = random.Next(70, 100),
                    Q3Score = random.Next(70, 100),
                    Q4Score = random.Next(70, 100),
                    Notes = "تم التوليد تلقائياً لاختبار أداء النظام بكثافة بيانات عالية."
                };
                evaluations.Add(eval);
            }
        }

        db.Evaluations.AddRange(evaluations);
        db.SaveChanges();

        // Seed Goals & Competencies for each evaluation
        foreach (var eval in evaluations)
        {
            // Goals
            var goalCount = random.Next(4, 7);
            var goals = new List<ObjectiveGoal>();
            for (int i = 1; i <= goalCount; i++)
            {
                goals.Add(new ObjectiveGoal
                {
                    EvaluationId = eval.Id,
                    GoalDescription = $"الهدف التشغيلي رقم {i} لعام {eval.Year}",
                    Weight = 65.0 / goalCount,
                    DisplayOrder = i,
                    Q1M1Target = 1, Q1M1Actual = random.NextDouble() * 1.2,
                    Q2M1Target = 1, Q2M1Actual = random.NextDouble() * 1.2,
                    Q3M1Target = 1, Q3M1Actual = random.NextDouble() * 1.2,
                    Q4M1Target = 1, Q4M1Actual = random.NextDouble() * 1.2
                });
            }
            db.Goals.AddRange(goals);

            // Competencies
            var comps = new List<CompetencyEvaluation>
            {
                new() { EvaluationId = eval.Id, Name = "الالتزام بالحضور والانصراف", Type = CompetencyType.Core, Weight = 10, Q1M1Rating = random.Next(2, 4), Q2M1Rating = random.Next(2, 4), Q3M1Rating = random.Next(2, 4), Q4M1Rating = random.Next(2, 4) },
                new() { EvaluationId = eval.Id, Name = "التمكن التقني والذكاء الاصطناعي", Type = CompetencyType.Skills, Weight = 5, Q1M1Rating = random.Next(2, 4), Q2M1Rating = random.Next(2, 4), Q3M1Rating = random.Next(2, 4), Q4M1Rating = random.Next(2, 4) },
                new() { EvaluationId = eval.Id, Name = "التعاون والمبادرة", Type = CompetencyType.Skills, Weight = 10, Q1M1Rating = random.Next(2, 4), Q2M1Rating = random.Next(2, 4), Q3M1Rating = random.Next(2, 4), Q4M1Rating = random.Next(2, 4) },
                new() { EvaluationId = eval.Id, Name = "الإبداع والابتكار", Type = CompetencyType.Innovation, Weight = 10, Q1M1Rating = random.Next(2, 4), Q2M1Rating = random.Next(2, 4), Q3M1Rating = random.Next(2, 4), Q4M1Rating = random.Next(2, 4) }
            };
            db.Competencies.AddRange(comps);
        }

        db.SaveChanges();
    }
}
