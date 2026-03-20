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
