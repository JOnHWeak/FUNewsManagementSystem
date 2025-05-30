using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO.Response
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }
}
