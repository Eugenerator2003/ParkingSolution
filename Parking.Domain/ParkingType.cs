using System;
using System.Collections.Generic;

namespace WebParking.Domain
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
