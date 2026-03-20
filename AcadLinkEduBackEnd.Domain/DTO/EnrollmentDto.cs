using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class EnrollmentDto
    {
        public int? Id { get; set; }
        public int? StudentId { get; set; }
        public int? ClassId { get; set; }
    }
}
