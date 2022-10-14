using Arch.EntityFrameworkCore;
using Arch.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Parking.Application;
using Parking.Domain;

public class Program
{
    public static void Main(string[] args)
    {
        using (var db = new ParkingContext())
        {
            var flag = true;
            while(flag)
            {
                Console.WriteLine("Выберите пункт меню");
                Console.WriteLine("1. Выборка из таблиц отношения <Один>.");
                Console.WriteLine("2. Выборка из таблиц отношения <Один> с некоторым фильтром.");
                Console.WriteLine("3. Выборка данных, сгруппированных по любому из полей данных с выводом какого-либо" +
                                  " итого-вого результата (min, max, avg, сount или др.) по выбранному полю из таблицы," +
                                  " стоящей в схеме базы данных нас стороне отношения <Многие>.");
                Console.WriteLine("4. Выборку данных из двух полей двух таблиц, связанных между собой отношением" +
                                  " <Один-ко-Многим>.");
                Console.WriteLine("5. Выборку данных из двух таблиц, связанных между собой отношением <Один-ко-Многим> и" +
                                  " от-фильтрованным по некоторому условию, налагающему ограничения на значения одного" +
                                  " или нескольких полей.");
                Console.WriteLine("6. Вставку данных в таблицы, стоящей на стороне отношения <Один>.");
                Console.WriteLine("7. Вставку данных в таблицы, стоящей на стороне отношения <Многие>.");
                Console.WriteLine("8. Удаление данных из таблицы, стоящей на стороне отношения <Один>.");
                Console.WriteLine("9. Удаление данных из таблицы, стоящей на стороне отношения <Многие>.");
                Console.WriteLine("10. Удаление данных из таблицы, стоящей на стороне отношения <Многие>.");
                Console.WriteLine("11. Выход");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        _Task1(db);
                        break;
                    case "2":
                        _Task2(db);
                        break;
                    case "3":
                        _Task3(db);
                        break;
                    case "4":
                        _Task4(db);
                        break;
                    case "5":
                        _Task5(db);
                        break;
                    case "6":
                        _Task6(db);
                        break;
                    case "7":
                        _Task7(db);
                        break;
                    case "8":
                        _Task8(db);
                        break;
                    case "9":
                        _Task9(db);
                        break;
                    case "10":
                        _Task10(db);
                        break;
                    case "11":
                        flag = false;
                        break;
                    default:
                        Console.WriteLine($"Нет варианта {option}");
                        break;
                }
                Console.WriteLine("Для продолжения нажмите любую клавишу");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    private static void _Task1(ParkingContext db)
    {
        var elements = db.CarMarks;
        Console.WriteLine("Марки машин");
        foreach(var e in elements)
        {
            Console.WriteLine($"{e.Id} - {e.Name}");
        }
    }

    private static void _Task2(ParkingContext db)
    {
        var elements = db.Owners.Where(o => o.Fullname.StartsWith("E"));
        Console.WriteLine("Владельцы машин, чьи имена начинаются с \"Е\"");
        foreach(var e in elements)
        {
            Console.WriteLine($"{e.Id} - {e.Fullname} - +{e.PhoneNumber}");
        }
    }

    private static void _Task3(ParkingContext db)
    {
        var elements = from c in db.Cars
                       group c by c.CarMarkId
                       into g
                       select new { CarMark = g.Key, Count = g.Count() };
        Console.WriteLine("Количество машин каждой марки по id");
        foreach(var e in elements)
        {
            Console.WriteLine($"{e.CarMark} - {e.Count}");
        }
    }

    private static void _Task4(ParkingContext db)
    {
        var elements = from c in db.Cars
                       join m in db.CarMarks
                       on c.CarMarkId equals m.Id
                       select new { Mark = m.Name, Number = c.Number };
        Console.WriteLine("Номера машин и их марки");
        foreach(var e in elements)
        {
            Console.WriteLine($"{e.Number} - {e.Mark}");
        }
    }

    private static void _Task5(ParkingContext db)
    {
        var elements = from c in db.Cars
                       join o in db.Owners
                       on c.OwnerId equals o.Id
                       where o.PhoneNumber.ToString().StartsWith("17")
                       select new { CarNumber = c.Number };
        Console.WriteLine("Номера машин, номера владельцев которых начинаются с 17");
        foreach(var e in elements)
        {
            Console.WriteLine($"{e.CarNumber}");
        }
    }
    
    private static void _Task6(ParkingContext db)
    {
        Console.WriteLine("Добавление марки машин");
        Console.WriteLine("Введите название марки машины");
        var markName = Console.ReadLine();
        db.CarMarks.Add(new CarMark { Name = markName });
        db.SaveChanges();
        Console.WriteLine("Марка машин была добавлена");
        foreach(var e in db.CarMarks.ToList().TakeLast(10))
        {
            Console.WriteLine($"{e.Id} - {e.Name}");
        }
    }

    public static void _Task7(ParkingContext db)
    {
        var random = new Random();
        var markId = random.Next(1, db.CarMarks.Count());
        var ownerId = random.Next(1, db.Owners.Count());
        Console.WriteLine("Добавление машины (со случайными владельцем и маркой)");
        Console.WriteLine("Введите номер машины");
        var number = Console.ReadLine();
        var car = new Car { OwnerId = ownerId, CarMarkId = markId, Number = number };
        db.Cars.Add(car);
        db.SaveChanges();
        Console.WriteLine("Машина была добавлена");
        foreach (var e in db.Cars.ToList().TakeLast(10))
        {
            Console.WriteLine($"{e.Id} - {e.OwnerId} - {e.Number}");
        }
    }

    public static void _Task8(ParkingContext db)
    {
        Console.WriteLine("Удаление марки машин");
        var mark = db.CarMarks.ToList().TakeLast(1).First();
        Console.WriteLine($"{mark.Id} - {mark.Name}");
        var cars = db.Cars.Where(c => c.CarMarkId == mark.Id);
        foreach(var car in cars)
        {
            db.Cars.Remove(car);
        }
        db.CarMarks.Remove(mark);
        db.SaveChanges();
        Console.WriteLine("Марка машин была удалена");
        foreach (var e in db.CarMarks.ToList().TakeLast(10))
        {
            Console.WriteLine($"{e.Id} - {e.Name}");
        }
    }

    public static void _Task9(ParkingContext db)
    {
        Console.WriteLine("Удаление машины");
        var car = db.Cars.ToList().TakeLast(1).First();
        Console.WriteLine($"{car.Id} - {car.OwnerId} - {car.Number}");
        db.Cars.Remove(car);
        db.SaveChanges();
        Console.WriteLine("Машина была удалена");
        foreach(var e in db.Cars.ToList().TakeLast(10))
        {
            Console.WriteLine($"{e.Id} - {e.OwnerId} - {e.Number}");
        }
    }

    public static void _Task10(ParkingContext db)
    {
        Console.WriteLine("Изменения номера владельца машин");
        var ownerId = Random.Shared.Next(1, db.Owners.Count());
        var owner = db.Owners.Where(o => o.Id == ownerId).First();
        Console.WriteLine($"До изменения:\n{owner.Id} - {owner.PhoneNumber}");
        owner.PhoneNumber = 312839810921;
        db.SaveChanges();
        var ownerUpdated = db.Owners.Where(o => o.Id == ownerId).First();
        Console.WriteLine($"После изменения:\n{ownerUpdated.Id} -- {ownerUpdated.PhoneNumber}");
    }
}