namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class SubmissionsDto
    {
        public int? Id { get; set; }

        public int? ActivityId { get; set; }

        public int? StudentId { get; set; }

        public string? FileUrl { get; set; } = null!;

        public string? FileName { get; set; } = null!;

        public string? Status { get; set; } = null!;

        public DateTime? SubmittedAt { get; set; }

    }
}
