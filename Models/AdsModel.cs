using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class AdsModel
    {
        public double ID { get; set; }

        public double UserID { get; set; }

        [RegularExpression(@"/\.(gif|jpe?g|tiff|png|webp|bmp)$/i", ErrorMessage = "It dosen't look like an image")]
        public HttpPostedFileBase Image { get; set; }

        [Display(Name = "Ad Image")]
        [Required]
        public string ImgUrl { get; set; }

        [Display(Name = "Target/On click redirect")]
        [Required]
        [Url(ErrorMessage = "Please enter a valid url")]
        public string Target { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [Display(Name = "Category")]
        [Required]
        public double CatID { get; set; }

        public long Views { get; set; }
        public string FormatedViews { get; set; }

        public long Clicks { get; set; }
        public string FormatedClicks { get; set; }

        [Required]
        public int NoOfDays { get; set; }
    }

    public class PaymentDetails
    {
        public double UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public double Amount { get; set; }

        public string Razor_Key { get; set; }
        public string Razor_payment_id { get; set; }
    }

    public class AdsAndPaymentBundle
    {
        public AdsModel Ads { get; set; }
        public PaymentDetails Pay { get; set; }
    }
}