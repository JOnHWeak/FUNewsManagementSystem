using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO.Response
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Role { get; set; }

    }
}
