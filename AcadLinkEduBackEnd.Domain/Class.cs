using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("classes")]
    public class Class : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TeacherId { get; set; }
        public string InviteCode { get; set; } = null!;
    }
}
