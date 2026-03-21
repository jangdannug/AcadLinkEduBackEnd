using Microsoft.AspNetCore.Http;

public class CreateSubmissionDto
{
    public int? ActivityId { get; set; }
    public int? StudentId { get; set; }

    public IFormFileCollection? Files { get; set; }
}