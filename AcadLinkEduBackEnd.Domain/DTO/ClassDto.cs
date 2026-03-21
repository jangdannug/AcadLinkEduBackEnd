namespace AcadLinkEduBackEnd.Application.Dtos;

public class ClassDto
{
    public int? Id { get; set; }
    public string? Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; } = null!;
    public string? InviteCode { get; set; } = null!;
    public bool IsEnrolled { get; set; }
    public ClassStats? Stats { get; set; }
}

public class ClassStats
{
    public int? Total { get; set; }
    public int? Submitted { get; set; }
    public int? Pending { get; set; }
}

public class AnalyticsDto
{
    public int TotalClasses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalSubmissions { get; set; }
    public int CompletionRate { get; set; }
}

public class ClassTrackingActivityDto
{
    public int? ActivityId { get; set; }
    public string? ActivityTitle { get; set; }
    public string? Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
}

public class ClassTrackingDto
{
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? StudentEmail { get; set; }
    public List<ClassTrackingActivityDto>? Activities { get; set; }
}
