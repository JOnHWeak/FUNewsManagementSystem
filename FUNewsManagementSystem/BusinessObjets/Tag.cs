using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects;

public partial class Tag
{
    [Key]
    public int TagId { get; set; }

    [Required]
    [StringLength(50)]
    public string? TagName { get; set; }

    [StringLength(200)]
    public string? Note { get; set; }

    public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
}

