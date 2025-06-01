using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class NewsArticle
{
    [Key]
    public string NewsArticleId { get; set; } = null!;

    [Required]
    [StringLength(200)]
    public string? NewsTitle { get; set; }

    [Required]
    [StringLength(500)]
    public string Headline { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    [StringLength(2000)]
    public string? NewsContent { get; set; }

    [StringLength(200)]
    public string? NewsSource { get; set; }

    public short? CategoryId { get; set; }

    public bool? NewsStatus { get; set; }

    public short? CreatedById { get; set; }

    public short? UpdatedById { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Category? Category { get; set; }
    public virtual SystemAccount? CreatedBy { get; set; }
    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}
