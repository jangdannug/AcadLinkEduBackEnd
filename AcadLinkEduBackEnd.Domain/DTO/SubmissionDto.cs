public class SubmissionDto
{
    public int? Id { get; set; }
    public int? ActivityId { get; set; }
    public int? StudentId { get; set; }

    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

    public string? Status { get; set; }
    public DateTime? SubmittedAt { get; set; }
}