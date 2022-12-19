using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebParking.Domain
{
    public partial class WorkShift
    {
        public int Id { get; set; }

        [Display(Name = "Start time")]
        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }

        [Display(Name = "End time")]
        [DataType(DataType.Time)]
        public DateTime? EndTime { get; set; }
        public int? EmploeyeeId { get; set; }
        public virtual Emploeyee? Employee { get; set; }
    }
}
