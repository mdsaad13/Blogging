using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blogging.Models
{
    public class BlogModel
    {
        public int BlogID { get; set; }

        public long UserID { get; set; }

        [Required]
        public long CatID { get; set; }

        [Required]
        [AllowHtml]
        [StringLength(2000, MinimumLength = 30, ErrorMessage = "Content should contain minimum 10 characters")]
        public string Content { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title should contain minimum 5 characters")]
        public string Title { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public int Status { get; set; }

        public long ViewCount { get; set; }

        public long ViewTime { get; set; }

        [Required]
        [RegularExpression("^([a-zA-Z0-9_.-]{5,30})$", ErrorMessage = "URL should be at least of 5 characters and can only include _ and . special characters")]
        public string URL { get; set; }

        public bool NextStep { get; set; }
    }
}