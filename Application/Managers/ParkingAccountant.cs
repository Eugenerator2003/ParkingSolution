using Parking.Domain.Models;
using Remotion.Linq.Parsing.ExpressionVisitors.MemberBindings;
using WebParking.Application;

namespace WebParking.Managers
{
    public class ParkingAccountant
    {
        private readonly decimal _salaryMultiply = 0.3m;
        
        public decimal SalaryMultiply { get { return _salaryMultiply; } }

        public ParkingAccountant() { }

        public ParkingAccountant(decimal salaryMultiply)
        {
            _salaryMultiply = salaryMultiply;
        }

        public decimal GetSalary(DateTime? month, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return GetPayments(month, parkingRecords, paymentTariffs) * SalaryMultiply;
        }

        public decimal GetEarnings(DateTime? month, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return GetPayments(month, parkingRecords, paymentTariffs) * (1 - SalaryMultiply);
        }

        public IEnumerable<(int, decimal)> GetEmployeesSalary(IEnumerable<Emploeyee> emploeyees, DateTime month, IEnumerable<WorkShift> workShifts, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return (from e in emploeyees
                   join s in GetPaymentForkWorkShifts(month, workShifts, parkingRecords, paymentTariffs)
                   on e.Id equals s.Item1.EmploeyeeId
                   group s by s.Item1.EmploeyeeId
                   into g
                   select (g.Key.Value, g.Sum(g => g.Item2) * _salaryMultiply)).ToList();
        }

        public IEnumerable<(int, decimal)> GetPaymentForWorkShiftsId(DateTime? month, IEnumerable<WorkShift> workShifts, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return GetPaymentForkWorkShifts(month, workShifts, parkingRecords, paymentTariffs).Select(s => (s.Item1.Id, s.Item2)).ToList();
        }

        public IEnumerable<(WorkShift, decimal)> GetPaymentForkWorkShifts(DateTime? month, IEnumerable<WorkShift> workShifts, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            var payments = parkingRecords.
                           Where(r => (month.HasValue ? 
                                       (r.DepartureTime.HasValue ? (r.DepartureTime.Value.Year == month.Value.Year &&
                                                                    r.DepartureTime.Value.Month == month.Value.Month)
                                                                : false)
                                                      : true)).
                           GroupBy(r => r.DepartureTime).
                           Select(g => new
                           {
                               Date = g.Key,
                               Payment = g.Sum(p => GetPayment(p, paymentTariffs))
                           }).ToList();

            workShifts = workShifts.Where(s => s.EndTime.HasValue).ToList();

            var list = new List<(WorkShift, decimal)>();

            foreach (var shift in workShifts)
            {
                var pay = payments.FirstOrDefault(p => p.Date.HasValue && p.Date.Value.Date == shift.StartTime.Value.Date);

                if (pay is not null)
                {
                    list.Add((shift, pay.Payment));
                }
                else
                {
                    list.Add((shift, 0));
                }

            }

            return list;
        }

        public IEnumerable<(DateTime?, decimal)> GetPaymentsForMonthDate(DateTime? month, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return parkingRecords.
                    Where(r => month.HasValue ?
                                     (r.DepartureTime.HasValue ?
                                                                (r.DepartureTime.Value.Year == month.Value.Year &&
                                                                r.DepartureTime.Value.Month == month.Value.Month) 
                                                               : false) 
                                              : true).
                    GroupBy(r => r.DepartureTime).
                    Select(g => (g.Key, g.Sum(p => GetPayment(p, paymentTariffs)))).ToList();
        }

        public decimal GetPayment(ParkingRecord parkingRecord, IEnumerable<PaymentTariff> paymentTariffs)
        {
            if (parkingRecord.DepartureTime == null) return 0;

            var tariff = paymentTariffs.FirstOrDefault(t => t.Id == parkingRecord.PaymentTariffIdId);

            if (tariff == null)
            {
                return 0;
            }

            if (tariff.DaysCount <= 3)
            {
                return tariff.Payment.Value;
            }

            var paymentOut = tariff.Payment.Value;
            var payment = tariff.Payment.Value * 0.9m;
            for(var i = 3; i <= tariff.DaysCount; i++)
            {
                paymentOut += payment;
            }

            return paymentOut;
        }

        private decimal GetPayments(DateTime? month, IEnumerable<ParkingRecord> parkingRecords, IEnumerable<PaymentTariff> paymentTariffs)
        {
            return GetPaymentsForMonthDate(month, parkingRecords, paymentTariffs).Sum(p => p.Item2);
        }
    }
}
