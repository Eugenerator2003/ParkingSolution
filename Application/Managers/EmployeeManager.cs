using Parking.Domain.Models;

namespace WebParking.Managers
{
    public class EmployeeManager
    {
        public IEnumerable<(int, int)> GetWorkShiftCountWithId(IEnumerable<WorkShift> workShifts, int month)
        {
            return workShifts.Where(s => s.StartTime.Value.Month == month).
                   GroupBy(s => s.EmploeyeeId).
                   Select(g => (g.Key.Value, g.Count())).ToList();
        }

        public IEnumerable<(int, int)> GetWorkShiftsCountWithIdThisMonth(IEnumerable<WorkShift> workShifts)
        {
            return GetWorkShiftCountWithId(workShifts, DateTime.Now.Month).ToList();
        }
    }
}
