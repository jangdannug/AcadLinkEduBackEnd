using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; } // Auto-increment integer
        [Column("email")]
        public string Email { get; set; } = null!;
        [Column("name")]
        public string Name { get; set; } = null!;
        [Column("role")]
        public string Role { get; set; } = null!;
        [Column("is_verified")]
        public bool IsVerified { get; set; }
    }
}
