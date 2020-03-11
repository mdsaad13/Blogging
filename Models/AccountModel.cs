using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Blogging.Models
{
    public class AccountModel
    {
        public long ID { get; set; }


        [Required(ErrorMessage = "What's your name?")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Enter a valid name")]
        public string Name { get; set; }
        

        [Required(ErrorMessage = "Kindly enter your email")]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string Email { get; set; }
        

        [Required(ErrorMessage = "You'll use this when you log in and if you ever need to reset your password.")]
        [Display(Name = "Mobile Number")]
        [RegularExpression("^([6-9]{1})([0-9]{9})$", ErrorMessage = "Enter a valid mobile number")]
        public string Mobile { get; set; }
        
        
        [Required(ErrorMessage = "You'll use this when you log in and if you ever need to reset your password.")]
        [Display(Name = "User Name")]
        [RegularExpression("^([a-zA-Z0-9_-]{5,30})$", ErrorMessage = "Username should be atleast of 5 characters and can only include _ and . special characters")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "This field connot be empty")]
        [Display(Name = "User Name or Mobile Number")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Enter a valid username or mobile number")]
        public string unameORmobile { get; set; }


        [Required(ErrorMessage = "Please choose a gender")]
        public string Gender { get; set; }
        

        [Required(ErrorMessage = "It looks like you've entered the wrong info. Please make sure that you use your real date of birth")]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }


        [Required(ErrorMessage = "Kindly enter your password")]
        [Display(Name = "Password")]
        [StringLength(16, MinimumLength = 6, ErrorMessage = "Enter a valid password")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z!#$%^&_+-=]{6,16}$", ErrorMessage = "Enter a valid password")]
        public string Password { get; set; }


        [Display(Name = "Re-Enter Password")]
        [Compare("Password", ErrorMessage = "Entered Password did not match")]
        public string ConfirmPass { get; set; }

        [Display(Name = "Profile picture")]
        public string ImgUrl { get; set; }


        [RegularExpression(@"([a-zA-Z0-9\s_\\.\-:])+(.png|.jpg|.gif|.jpeg)$", ErrorMessage = "It dosen't look like an image")]
        public HttpPostedFileBase Image { get; set; }
    }

    public class ProfileModel
    {
        public double UserID { get; set; }
        public string Name { get; set; }
        public string Uname { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string ImgURL { get; set; }
        public string DOB { get; set; }
        public string Followers { get; set; }
        public string Following { get; set; }
        public string BlogsCount { get; set; }
    }
}