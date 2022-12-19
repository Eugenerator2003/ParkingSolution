using System.ComponentModel.DataAnnotations;
using WebParking.Domain;

namespace WebParking.ViewModels
{
    public class CarOwnersViewModel
    {
        public IEnumerable<Owner> Owners { get; set; }

        [Display(Name = "Full name")]
        public string Fullname { get; set; }

        [Display(Name = "Phone number")]
        public long? PhoneNumber { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
