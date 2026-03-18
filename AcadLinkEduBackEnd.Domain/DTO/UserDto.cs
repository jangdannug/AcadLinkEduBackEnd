using System;
using System.Collections.Generic;
using System.Text;

namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;
        public bool IsVerified { get; set; }
    }
}
