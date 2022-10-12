using System;
using System.Collections.Generic;

namespace Parking.Domain
{
    public partial class Owner
    {
        public Owner()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string? Fullname { get; set; }
        public long? PhoneNumber { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
