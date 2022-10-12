using System;
using System.Collections.Generic;

namespace Parking.Domain
{
    public partial class WorkShift
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? EmploeyeeId { get; set; }

        public virtual Emploeyee? Emploeyee { get; set; }
    }
}
