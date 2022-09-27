using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parking.Domain
{
    internal class Owners
    {
        public int Id { get; set; }

        public string Fullname { get; set; } = "";

        public long PhoneNumber { get; set; }
    }
}
