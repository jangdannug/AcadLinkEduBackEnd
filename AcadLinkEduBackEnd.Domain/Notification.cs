using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("notifications")]
    public class Notification : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("title")]
        public string Title { get; set; } = null!;
        [Column("message")]
        public string Message { get; set; } = null!;
        [Column("type")]
        public string Type { get; set; } = null!;
        [Column("is_read")]
        public bool IsRead { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
      
    }
}
