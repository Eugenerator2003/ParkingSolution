using System.ComponentModel.DataAnnotations;

namespace WebParking.ViewModels
{
    public class EarningBillViewModel
    {
        [Display(Name = "Salary: ")]
        public decimal? Salary { get; set; }

        [Display(Name = "Earning: ")]
        public decimal? Earning { get; set; }

        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
