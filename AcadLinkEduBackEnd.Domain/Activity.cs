using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("activities")]
    public class Activity : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        public string[] RequiredFiles { get; set; } = Array.Empty<string>();
    }
}
