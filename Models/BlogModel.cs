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


        [Display(Name = "Category")]
        [Required]
        public long CatID { get; set; }


        [Required]
        [AllowHtml]
        [StringLength(2000, MinimumLength = 30, ErrorMessage = "Content should contain minimum 10 characters")]
        public string Content { get; set; }


        [Display(Name = "Title of your blog")]
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title should contain minimum 5 characters")]
        public string Title { get; set; }

        public DateTime DateTime { get; set; }

        public DateTime UpdateDateTime { get; set; }

        public int Status { get; set; }

        public long ViewCount { get; set; }
        public string FormatViewCount { get; set; }

        public long ViewTime { get; set; }
        public string FormatViewTime { get; set; }


        [Display(Name = "URL to access your blog")]
        [Required]
        [RegularExpression("^([a-zA-Z0-9_-]{5,30})$", ErrorMessage = "URL should be at least of 5 characters and can only include _ and - special characters")]
        public string URL { get; set; }


        [Display(Name = "Add images to blog")]
        public bool NextStep { get; set; }

        public long Likes { get; set; }
        public long Comments { get; set; }
    }

    public class BlogImages
    {
        public int ID { get; set; }

        public int BlogID { get; set; }

        public string URL { get; set; }

        public string Alt { get; set; }

    }

    public class BlogBundle
    {
        public BlogModel Blog { get; set; }
        public List<BlogImages> BlogImages { get; set; }
        public List<CommentsModel> Comments { get; set; }
    }
}