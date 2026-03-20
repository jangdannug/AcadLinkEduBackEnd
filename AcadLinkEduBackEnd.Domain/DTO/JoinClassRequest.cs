using System;
using System.Collections.Generic;
using System.Text;

namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class JoinClassRequest
    {
        public int? StudentId { get; set; }
        public string? InviteCode { get; set; } = string.Empty;
    }
}
