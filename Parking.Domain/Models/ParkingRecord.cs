using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking.Domain.Models
{
    public partial class ParkingRecord
    {
        public int Id { get; set; }
        [Display(Name = "Car")]
        public int? CarId { get; set; }
        [Display(Name = "Entry time")]
        public DateTime? EntryTime { get; set; }
        [Display(Name = "Departure time")]
        public DateTime? DepartureTime { get; set; }
        [Display(Name = "Parking type")]
        public int? PaymentTariffIdId { get; set; }
        public int? EmployeeId { get; set; }
        [Display(Name = "Car")]
        public virtual Car? Car { get; set; }
        public virtual Emploeyee? Employee { get; set; }

        [Display(Name = "Parking type")]
        public virtual PaymentTariff? PaymentTariffId { get; set; }
    }
}
