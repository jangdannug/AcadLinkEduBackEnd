using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;

namespace AcadLinkEduBackEnd.Domain.Entities
{
    [Table("enrollments")]
    public class Enrollment : BaseModel
    {
        [PrimaryKey("id")]
        public int? Id { get; set; }
        [Column("student_id")]
        public int? StudentId { get; set; }
        [Column("class_id")]
        public int? ClassId { get; set; }
    }
}
