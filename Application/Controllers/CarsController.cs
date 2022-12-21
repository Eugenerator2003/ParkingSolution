using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Parking.Domain.Models;
using WebParking.Application;
using WebParking.ViewModels;

namespace WebParking.Controllers
{
    [Authorize()]
    public class CarsController : Controller
    {
        private readonly ParkingContext _context;
        private const int _pageSize = 20;
        private const string modelName = "CarsViewModel";
        private CacheProvider _cache;

        public CarsController(ParkingContext context, CacheProvider cache)
        {
            _context = context;
            _cache = cache;
        }

        private IEnumerable<T> PushDefault<T>(IEnumerable<T> values) where T: new()
        {
            var list = values.ToList();
            list.Insert(0, new T() { });
            return list;
        }

        // GET: Cars
        public async Task<IActionResult> Index(string? Number, int? CarMarkId, int? OwnerId,  string OldFieldName, string FieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            Number = CookieProccesor.GetSetValue("CarNumber", Number ?? "", first, Request, Response);
            CarMarkId = CookieProccesor.GetSetValue<int>("CarMarkId", CarMarkId.HasValue ? CarMarkId.Value.ToString() : "", first, Request, Response);
            OwnerId = CookieProccesor.GetSetValue<int>("CarMarkId", OwnerId.HasValue ? OwnerId.Value.ToString() : "", first, Request, Response);

            var viewModel = _cache.GetItem<CarsViewModel>(modelName);

            if (viewModel != null && viewModel.CarMarkId == CarMarkId && viewModel.OwnerId == OwnerId &&
                viewModel.PageViewModel.PageNumber == page && viewModel.Number == Number &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName))
            {
                return View(viewModel);
            }

            IEnumerable<Car> parkingContext = Search(_context.Cars.Include(c => c.CarMark).Include(c => c.Owner), Number,
                                                     CarMarkId ?? 0, OwnerId ?? 0);

            var count = parkingContext.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            parkingContext = Sort(parkingContext, sortViewModel).Skip((page - 1) * _pageSize).Take(_pageSize);

            viewModel = new CarsViewModel()
            {
                CarMarkId = CarMarkId,
                OwnerId = OwnerId,
                Number = Number,
                Cars = parkingContext,
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                Owners = new SelectList(PushDefault(_context.Owners), "Id", "Fullname", OwnerId),
                CarMarks = new SelectList(PushDefault(_context.CarMarks), "Id", "Name", CarMarkId)
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMark)
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewData["CarMarkId"] = new SelectList(_context.CarMarks, "Id", "Name");
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Fullname");
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,CarMarkId,OwnerId")] Car car)
        {
            if (ModelState.IsValid)
            {
                _context.Add(car);
                await _context.SaveChangesAsync();
                _cache.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarMarkId"] = new SelectList(_context.CarMarks, "Id", "Name", car.CarMarkId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Fullname", car.OwnerId);
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["CarMarkId"] = new SelectList(_context.CarMarks, "Id", "Name", car.CarMarkId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Fullname", car.OwnerId);
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,CarMarkId,OwnerId")] Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    _cache.SetItem(null, modelName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarMarkId"] = new SelectList(_context.CarMarks, "Id", "Name", car.CarMarkId);
            ViewData["OwnerId"] = new SelectList(_context.Owners, "Id", "Fullname", car.OwnerId);
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Cars == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.CarMark)
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Cars == null)
            {
                return Problem("Entity set 'ParkingContext.Cars'  is null.");
            }
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                _cache.SetItem(null, modelName);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
          return _context.Cars.Any(e => e.Id == id);
        }

        private IEnumerable<Car> Search(IQueryable<Car> cars, string number, int carMakrId, int ownerId)
        {
            return cars.Where(c => c.Number.Contains(number) &&
                              (carMakrId > 0 ? c.CarMarkId == carMakrId : true) &&
                              (ownerId > 0 ? c.OwnerId == ownerId : true));
        }

        private List<Car> Sort(IEnumerable<Car> cars, SortViewModel sortViewModel)
        {
            Func<Car, object> func = null;

            switch (sortViewModel.FieldName)
            {
                case "Number":
                    func = f => f.Number;
                    break;
                case "CarMarkId":
                    func = f => f.CarMark.Name;
                    break;
                case "OwnerId":
                    func = f => f.Owner.Fullname;
                    break;
                default:
                    func = f => f.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    cars = cars.OrderBy(func);
                    break;
                case SortState.Descending:
                    cars = cars.OrderByDescending(func);
                    break;
            }

            return cars.ToList();
        }
    }
}
