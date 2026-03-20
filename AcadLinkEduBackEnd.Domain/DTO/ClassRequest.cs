using System;
using System.Collections.Generic;
using System.Text;

namespace AcadLinkEduBackEnd.Domain.DTO
{
    public class ClassRequest
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int TeacherId { get; set; }
    }
}
