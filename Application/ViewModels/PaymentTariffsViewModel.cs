using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using WebParking.Domain;

namespace WebParking.ViewModels
{
    public class PaymentTariffsViewModel
    {
        [Display(Name = "Days count")]
        public int? DaysCount { get; set; }

        [Display(Name = "Payment")]
        public decimal? Payment { get; set; }

        [Display(Name = "Parking type")]
        public int? ParkingTypeId { get; set; }

        public IEnumerable<PaymentTariff> PaymentTariffs { get; set; }

        public SelectList ParkingTypes { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
