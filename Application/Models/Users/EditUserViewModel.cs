using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebParking.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Email address not specified")]
        [EmailAddress(ErrorMessage = "Incorrect email address")]
        [Display(Name = "Email addrss")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password not specified")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Field {0} must contain minimum {2} and maximum chars {1} символов.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string NewPassword { get; set; }


        [Required(ErrorMessage = "Old password not specified")]
        [Display(Name = "Old password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

    }
}
