using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO.Request
{
    public class CreateAccountRequest
    {
        [Required]
        [StringLength(100)]
        public string? AccountName { get; set; } // Nullable to match model

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string? AccountEmail { get; set; } // Nullable to match model

        [Required]
        [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
        public int? AccountRole { get; set; } // Nullable to match model

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string? AccountPassword { get; set; } // Nullable to match model
    }
}
