using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class NewsTag
    {
        [Required]
        public string NewsArticleId { get; set; }

        [Required]
        public int TagId { get; set; }

        public virtual NewsArticle NewsArticle { get; set; }
        public virtual Tag Tag { get; set; }
    }

}
