using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("activities")]
    public class Activity : BaseModel
    {
        [PrimaryKey("id")]
        public int? Id { get; set; }
        [Column("class_id")]
        public int? ClassId { get; set; }
        [Column("title")]
        public string? Title { get; set; } = null!;
        [Column("description")]
        public string? Description { get; set; } = null!;
        [Column("deadline")]
        public DateTime? Deadline { get; set; }
        [Column("required_files")]
        public string[] RequiredFiles { get; set; } = Array.Empty<string>();
    }
}
