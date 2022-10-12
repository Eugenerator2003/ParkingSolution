using System;
using System.Collections.Generic;

namespace Parking.Domain
{
    public partial class ParkingRecord
    {
        public int Id { get; set; }
        public int? CarId { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public int? PaymentTariffIdId { get; set; }
        public int? EmployeeId { get; set; }

        public virtual Car? Car { get; set; }
        public virtual Emploeyee? Employee { get; set; }
        public virtual PaymentTariff? PaymentTariffId { get; set; }
    }
}
