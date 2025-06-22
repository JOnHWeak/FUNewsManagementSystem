using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class TagResponseDto
    {
        public int TagId { get; set; }
        public string TagName { get; set; } = null!;
    }
}
