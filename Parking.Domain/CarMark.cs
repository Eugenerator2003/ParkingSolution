using System;
using System.Collections.Generic;

namespace WebParking.Domain
{
    public partial class CarMark
    {
        public CarMark()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
