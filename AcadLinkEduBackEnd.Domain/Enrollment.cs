using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("enrollments")]
    public class Enrollment : BaseModel
    {
        [PrimaryKey("id")]
        public int? Id { get; set; }
        public int StudentId { get; set; }
        public int? ClassId { get; set; }
    }
}
