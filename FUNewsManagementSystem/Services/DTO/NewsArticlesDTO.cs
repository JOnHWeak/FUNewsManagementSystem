using System;
using System.Collections.Generic;

namespace Services.DTO
{
    // Example simplified DTOs (adjust as needed)

    namespace Services.DTO
    {
        public class NewsArticleRequestDto
        {
            public string? NewsTitle { get; set; }
            public string Headline { get; set; } = null!;
            public string? NewsContent { get; set; }
            public string? NewsSource { get; set; }
            public short? CategoryId { get; set; }
            public bool? NewsStatus { get; set; }
            public short? CreatedById { get; set; }
            public short? UpdatedById { get; set; }
            public List<string>? TagIds { get; set; }
        }

        public class NewsArticleResponseDto
        {
            public string NewsArticleId { get; set; } = null!;
            public string? NewsTitle { get; set; }
            public string Headline { get; set; } = null!;
            public DateTime? CreatedDate { get; set; }
            public string? NewsContent { get; set; }
            public string? NewsSource { get; set; }
            public short? CategoryId { get; set; }
            public string? CategoryName { get; set; }
            public bool? NewsStatus { get; set; }
            public short? CreatedById { get; set; }
            public string? CreatedByName { get; set; }
            public short? UpdatedById { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public List<string>? Tags { get; set; }
        }
    }

}
