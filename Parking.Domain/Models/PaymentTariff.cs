using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking.Domain.Models
{
    public partial class PaymentTariff
    {
        public PaymentTariff()
        {
            ParkingRecords = new HashSet<ParkingRecord>();
        }

        public int Id { get; set; }

        public int? ParkingTypeId { get; set; }


        [Display(Name = "Days count")]
        public int? DaysCount { get; set; }
        public decimal? Payment { get; set; }

        [Display(Name = "Parking type")]
        public virtual ParkingType? ParkingType { get; set; }
        public virtual ICollection<ParkingRecord> ParkingRecords { get; set; }
    }
}
