using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Services.DTO
{
    // Example simplified DTOs (adjust as needed)

    namespace Services.DTO
    {
        public class NewsArticleRequestDto
        {
            [Required]
            [StringLength(200)]
            public string NewsTitle { get; set; } = null!;

            [Required]
            [StringLength(500)]
            public string Headline { get; set; } = null!;

            [StringLength(2000)]
            public string? NewsContent { get; set; }

            [StringLength(200)]
            public string? NewsSource { get; set; }

            public short? CategoryId { get; set; }
            public bool? NewsStatus { get; set; } = true;
            public int? CreatedById { get; set; }
            public int? UpdatedById { get; set; }

            // Support both tag names and IDs
            public List<string>? TagNames { get; set; }
            public List<int>? TagIds { get; set; }
        }

        public class NewsArticleResponseDto
        {
            public string NewsArticleId { get; set; } = null!;
            public string NewsTitle { get; set; } = null!;
            public string Headline { get; set; } = null!;
            public DateTime? CreatedDate { get; set; }
            public string? NewsContent { get; set; }
            public string? NewsSource { get; set; }
            public short? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public bool? NewsStatus { get; set; }
            public int? CreatedById { get; set; }
            public string? CreatedByName { get; set; }
            public int? UpdatedById { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public List<TagResponseDto>? Tags { get; set; }
        }
    }

}
