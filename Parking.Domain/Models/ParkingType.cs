using System;
using System.Collections.Generic;

namespace Parking.Domain.Models
{
    public partial class ParkingType
    {
        public ParkingType()
        {
            PaymentTariffs = new HashSet<PaymentTariff>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<PaymentTariff> PaymentTariffs { get; set; }
    }
}
