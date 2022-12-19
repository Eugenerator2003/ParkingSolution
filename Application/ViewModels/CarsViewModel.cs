using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebParking.Domain;

namespace WebParking.ViewModels
{
    public class CarsViewModel
    {
        [Display(Name = "Onwer")]
        public int? OwnerId { get; set; }

        [Display(Name = "Mark")]
        public int? CarMarkId { get; set; }

        [Display(Name = "Number")]
        public string Number { get; set; }

        public IEnumerable<Car> Cars { get; set; }

        public SelectList Owners { get; set; }

        public SelectList CarMarks { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
