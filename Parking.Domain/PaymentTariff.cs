using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Domain
{
    internal class PaymentTariff
    {
        public int Id { get; set; }

        public int ParkingTypeId { get; set; }

        public int DaysCount { get; set; }

        public decimal Payment { get; set; }
    }
}
