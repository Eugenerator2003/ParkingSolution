using System.Collections;
using Parking.Domain.Models;

namespace WebParking.Managers
{
    public class OwnerManager
    {

        public IEnumerable<Owner> GetRegularOwners(IEnumerable<ParkingRecord> records, DateTime monthOnly)
        {
            records = from record in records
                      where record.EntryTime.Value.Month == monthOnly.Month &&
                            record.EntryTime.Value.Year == monthOnly.Year
                      select record;

            return from g in (from record in records
                              group record by record.Car.Owner
                              into grp
                              select grp)
                   where g.Count() >= 5
                   select g.Key;
                   
        }
    }
}
