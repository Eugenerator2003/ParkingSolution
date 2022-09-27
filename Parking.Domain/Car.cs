using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Domain
{
    internal class Car
    {
        public int Id { get; set;}

        public int CarMarkId { get; set; }

        public int OwnerId { get; set; }
    }
}
