using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("submissions")]
    public class Submission : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int StudentId { get; set; }
        public string FileUrl { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; } = null!;
    }
}
