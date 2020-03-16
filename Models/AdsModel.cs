using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class AdsModel
    {
        public int ID { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        [Required]
        public string Target { get; set; }

        [Required]
        public DateTime FromDate { get; set; }
        public string FromDateString { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
        public string ToDateString { get; set; }

        [Required]
        public double CatID { get; set; }
    }
}