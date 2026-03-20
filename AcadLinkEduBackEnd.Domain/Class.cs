using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("classes")]
    public class Class : BaseModel
    {
        [PrimaryKey("id")]
        public int? Id { get; set; }
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("description")]
        public string Description { get; set; } = null!;
        [Column("teacher_id")]
        public int TeacherId { get; set; }
        [Column("invite_code")]
        public string InviteCode { get; set; } = null!;
    }
}
