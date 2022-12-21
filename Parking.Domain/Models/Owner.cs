using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Parking.Domain.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Cars = new HashSet<Car>();
        }

        public int Id { get; set; }
        [Display(Name = "Full name")]
        public string? Fullname { get; set; }

        [Display(Name = "Phone number")]
        public long? PhoneNumber { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
