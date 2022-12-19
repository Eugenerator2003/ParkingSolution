using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Collections.Specialized.BitVector32;


namespace WebParking.Models.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email address not specified")]
        [EmailAddress(ErrorMessage = "Incorrect email address")]
        [Display(Name = "Email addrss")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password not specified")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Field {0} must contain minimum {2} and maximum chars {1} символов.", MinimumLength = 6)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
