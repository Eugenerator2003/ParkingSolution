using Parking.Application;
using Parking.Domain;
using System;
using System.Runtime.InteropServices;

namespace ParkingWebApplication.Services.DbInitializer
{
    public class DbInitializer
    {

        private static Random _random = new Random();
        private static string[] _firstNames = { "Иван", "Александр", "Пётр", "Андрей", "Владислав", "Мария", "Надежда",
                                                "Светлана", "Екатерина", "Олег", "Ева", "Артём", "Павел", "Роман",
                                                "Алина", "Анна", "София", "Иван", "Максим" };
        private static string[] _secondNames = { "Постарнак", "Малащенко", "Морозько", "Маслак", "Помин", "Радченко",
                                                 "Акуленко", "Шагал", "Мищенко", "Протченко", "Шило", "Кудренко",
                                                 "Ганюченко", "Пипченко", "Калиниченко"};
        private static string[] _alphabet =
        {
            "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У",
            "Ф", "Х", "Ц", "Ч", "Ш", "Щ", "Ъ", "Ы", "Ь", "Э", "Ю", "Я"
        };

        private static T GetRandom<T>(IEnumerable<T> values, Random random)
        {
            return values.ElementAt(random.Next(0, values.Count()));
        }

        private static string GetName(Random random)
        {
            return GetRandom(_firstNames, random) + " " + GetRandom(_secondNames, random);
        }

        private static void _CreateMarks(ParkingContext db, int count)
        {
            if (db.CarMarks.Any()) return;
            string[] manufacture = { "Toyta", "Volkswagen", "BMW", "Lada", "Audi", "Mazda",
                                     "Honda", "Renault", "Mitsubishi", "Nissan", "Mersedes", "Subaru",
                                     "Lexus", "Opel", "Citroen", "Ferrari", "Pegeout", "Rover", "Maybach", };
            for(var i = 0; i < count; i++)
            {
                var random = Random.Shared;
                var code = random.Next(0, 1000000000);
                db.CarMarks.Add(new CarMark() { Name = GetRandom(manufacture, random) + code.ToString()});
            }
            db.SaveChanges();
        }

        private static void _CreateOwners(ParkingContext db, int count)
        {
            if (db.Owners.Any()) return;
            var random = Random.Shared;
            for (var i = 0; i < count; i++)
            {
                var numberStart = 375010000000;
                var numberEnd = 37599999999999;

                var phoneNum = random.NextInt64(numberStart, numberEnd);

                var name = GetName(random);

                db.Owners.Add(new Owner() { PhoneNumber = phoneNum, Fullname = name });
            }
            db.SaveChanges();
        }

        private static void _CreateCars(ParkingContext db, int count, int marksCount, int ownersCount)
        {
            if (db.Cars.Any()) return;
            var random = Random.Shared;
            for (var i = 0; i < count; i++)
            {
                var markId = random.Next(1, marksCount + 1);
                var ownerId = random.Next(1, ownersCount + 1);

                var number = random.Next(1000, 9999).ToString() + GetRandom(_alphabet, random)
                             + GetRandom(_alphabet, random) + random.Next(0, 8).ToString();

                db.Cars.Add(new Car() { CarMarkId = markId, OwnerId = ownerId, Number =  number});
            }
            db.SaveChanges();
        }

        private static void _CreateEmployees(ParkingContext db, int count)
        {
            if (db.Emploeyees.Any()) return;
            var random = Random.Shared;

            for(var i = 0; i < count; i++)
            {
                db.Emploeyees.Add(new Emploeyee() { Fullname = GetName(random)});
            }
            db.SaveChanges();
        }

        private static void _CreateParkingTypes(ParkingContext db, int count)
        {
            if (db.ParkingTypes.Any()) return;
            string[] types = { "Улица", "Здание"};

            for(var i = 0; i < count; i++)
            {
                db.ParkingTypes.Add(new ParkingType() { Name = types[i]});
            }
            db.SaveChanges();
        }

        private static void _CreatePaymentTariffs(ParkingContext db, int typesCount, int countToTypes)
        {
            if (db.PaymentTariffs.Any()) return;
            var random = Random.Shared;
            for (var i = 0; i < typesCount; i++)
            {
                var basePayment = random.Next(10, 100);
                for(var j = 0; j < countToTypes; j++)
                {
                    var payment = basePayment + random.Next(10, 40);
                    db.PaymentTariffs.Add(new PaymentTariff() { ParkingTypeId = i + 1, DaysCount = j, Payment = payment}) ;
                    basePayment = payment;
                }
            }
            db.SaveChanges();
        }

        private static void _CreateWorkShifts(ParkingContext db, int count, int employeesCount, DateTime dateStart, DateTime dateEnd)
        {
            if (db.WorkShifts.Any()) return;
            var random = Random.Shared;
            var diff = dateEnd.Subtract(dateStart).Days;
            for (var i = 0; i < count; i++)
            {
                var employeeId = random.Next(1, employeesCount + 1);
                var startDate = dateStart.AddDays(random.Next(0, diff));
                var endDate = dateStart.AddHours(12);
                if (random.Next(0, 2) == 1) { startDate.AddHours(12); endDate.AddHours(12); };
                db.WorkShifts.Add(new WorkShift()
                {
                    EmploeyeeId = employeeId,
                    StartTime = startDate,
                    EndTime = endDate
                });
            }
            db.SaveChanges();
        }

        private static void _CreateParkingRecords(ParkingContext db, int count, int employeesCount, int carsCount,
                                                  int tariffsCount, DateTime dateStart, DateTime dateEnd)
        {
           if (db.ParkingRecords.Any()) return;
            var random = Random.Shared;
            var diff = dateEnd.Subtract(dateStart).Days;
            for (var i = 0; i < count; i++)
            {
                var employeeId = random.Next(1, employeesCount + 1);
                var carId = random.Next(1, carsCount + 1);
                var startDate = dateStart.AddDays(random.Next(0, diff));
                DateTime? endDate = random.Next(0, 2) == 1 ? startDate.AddDays(random.Next(0, 10)) : null;
                int tariffId = random.Next(1, tariffsCount + 1);
                db.ParkingRecords.Add(new ParkingRecord()
                {
                    EmployeeId = employeeId,
                    CarId = carId,
                    EntryTime = startDate,
                    DepartureTime = endDate,
                    PaymentTariffIdId = tariffId
                }); ;
            }
            db.SaveChanges();
        }


        public static void Initialize(ParkingContext db)
        {
            db.Database.EnsureCreated();

            var marksCount = 20;
            var ownersCount = 20;
            var carsCount = 400;
            var employeesCount = 10;
            var typesCount = 2;
            var tariffsCount = 3;
            var shiftsCount = 200;
            var recordsCount = 100;

            var dateStart = DateTime.Now;
            var dateEnd = dateStart.AddYears(1);
  
            _CreateMarks(db, marksCount);
            _CreateOwners(db, ownersCount);
            _CreateCars(db, carsCount, marksCount, ownersCount);
            _CreateEmployees(db, employeesCount);
            _CreateParkingTypes(db, typesCount);
            _CreatePaymentTariffs(db, typesCount, 4);
            _CreateWorkShifts(db, shiftsCount, employeesCount, dateStart, dateEnd);
            _CreateParkingRecords(db, recordsCount, employeesCount, carsCount, tariffsCount, dateStart, dateEnd);

            db.SaveChanges();
        }   
    }
}
