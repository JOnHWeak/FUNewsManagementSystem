using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class SystemAccount
{
    [Key]
    public short AccountId { get; set; }

    [Required]
    [StringLength(100)]
    public string? AccountName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string? AccountEmail { get; set; }

    [Required]
    [Range(1, 2, ErrorMessage = "Role must be 1 (Staff) or 2 (Lecturer)")]
    public int? AccountRole { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string? AccountPassword { get; set; }

    public virtual ICollection<NewsArticle> NewsArticles { get; set; } = new List<NewsArticle>();
}