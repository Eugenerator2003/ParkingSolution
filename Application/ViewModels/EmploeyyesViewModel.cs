using System.ComponentModel.DataAnnotations;
using WebParking.Domain;

namespace WebParking.ViewModels
{
    public class EmploeyeesViewModel
    {
        [Display(Name = "Full name")]
        public string Fullname { get; set; }

        [Display(Name = "Shifts count this month")]
        public int ShiftsCount { get; set; }

        [Display(Name = "Salary this month")]
        public decimal Salary { get; set; }

        public IEnumerable<(Emploeyee, int, decimal)> Emploeyees { get; set; }

        public SortViewModel SortViewModel { get; set; }

        public PageViewModel PageViewModel { get; set; }
    }
}
