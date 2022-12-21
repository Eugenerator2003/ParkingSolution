using Microsoft.AspNetCore.Mvc.Rendering;
using Parking.Domain.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebParking.ViewModels
{
    public class WorkShiftsViewModel
    {
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End time")]
        public DateTime EndTime { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        public bool? SearchForDate { get; set; }

        public IEnumerable<(WorkShift, decimal)> WorkShifts { get; set; }

        public SelectList Employees { get; set; }

        public PageViewModel PageViewModel { get; set; }

        public SortViewModel SortViewModel { get; set; }
    }
}
