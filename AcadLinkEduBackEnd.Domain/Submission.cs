using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("submissions")]
    public class Submission : BaseModel
    {
        [PrimaryKey("id")]
        public int? Id { get; set; }
        
        [Column("activity_id")]
        public int? ActivityId { get; set; }
        
        [Column("student_id")]
        public int? StudentId { get; set; }
        
        [Column("file_url")]
        public string? FileUrl { get; set; } = null!;
        
        [Column("file_name")]
        public string? FileName { get; set; } = null!;
        
        [Column("status")]
        public string? Status { get; set; } = null!;

        [Column("submitted_at")]
        public DateTime? SubmittedAt { get; set; }
     
    }
}
