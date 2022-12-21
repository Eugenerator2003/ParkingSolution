using Microsoft.AspNetCore.Mvc.Rendering;
using Parking.Domain.Models;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WebParking.ViewModels
{
    public class ParkingRecordsViewModel
    {
        public bool SearchForEntryDate { get; set; }

        [Display(Name = "Entry date")]
        [DataType(DataType.Date)]
        public DateTime? EntryDate { get; set; }

        public bool SearchForDepatureDate { get; set; }

        [Display(Name = "Departure date")]
        [DataType(DataType.Date)]
        public DateTime? DepartureDate { get; set; }

        [Display(Name = "Car")]
        public int CarId { get; set; }

        [Display(Name = "Mark")]
        public int CarMarkId { get; set; }

        [Display(Name = "Owner number")]
        public string OwnerNumber { get; set; }

        [Display(Name = "Payment")]
        public decimal Payment { get; set; }

        [Display(Name = "Owner")]
        public int OwnerId { get; set; }

        [Display(Name = "Parking type")]
        public int? ParkingTypeId { get; set; }

        public IEnumerable<(ParkingRecord, decimal)> ParkingRecords { get; set; }

        public SelectList Cars { get; set; }

        public SelectList Onwers { get; set; }

        public SelectList ParkingTypes { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
