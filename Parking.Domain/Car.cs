using System;
using System.Collections.Generic;

namespace Parking.Domain
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

        public virtual CarMark? CarMark { get; set; }
        public virtual Owner? Owner { get; set; }
        public virtual ICollection<ParkingRecord> ParkingRecords { get; set; }
    }
}
