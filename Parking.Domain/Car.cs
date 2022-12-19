using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebParking.Domain
{
    public partial class Car
    {
        public Car()
        {
            ParkingRecords = new HashSet<ParkingRecord>();
        }

        public int Id { get; set; }
        public string? Number { get; set; }

        public int? CarMarkId { get; set; }

        public int? OwnerId { get; set; }

        [Display(Name = "Car mark")]
        public virtual CarMark? CarMark { get; set; }
        [Display(Name = "Owner full name")]
        public virtual Owner? Owner { get; set; }
        public virtual ICollection<ParkingRecord> ParkingRecords { get; set; }
    }
}
