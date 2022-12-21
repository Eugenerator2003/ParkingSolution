using System;
using System.Collections.Generic;

namespace Parking.Domain.Models
{
    public partial class Emploeyee
    {
        public Emploeyee()
        {
            ParkingRecords = new HashSet<ParkingRecord>();
            WorkShifts = new HashSet<WorkShift>();
        }

        public int Id { get; set; }
        public string? Fullname { get; set; }

        public virtual ICollection<ParkingRecord> ParkingRecords { get; set; }
        public virtual ICollection<WorkShift> WorkShifts { get; set; }
    }
}
