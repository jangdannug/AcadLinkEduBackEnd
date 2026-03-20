using System;
using System.Collections.Generic;
using System.Text;

namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class ActivityDto
    {
        public int? Id { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime? Deadline { get; set; }
        public string[] RequiredFiles { get; set; } = Array.Empty<string>();
    }
}
