using System;
using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Email")]
        public string  Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength( 6, ErrorMessage ="Password must be 6 character long!")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage =" Password and its confirmation do not match")]
        public string ConfirmPassword { get; set; }

        public string Birthdate => DateTime.Now.ToString("MM/dd/yyyy");
    }
}
