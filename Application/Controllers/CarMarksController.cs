global using WebParking.Utils;
global using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebParking.Application;
using WebParking.ViewModels;
using WebParking.CacheService;
using Parking.Domain.Models;

namespace WebParking.Controllers
{
    [Authorize()]
    public class CarMarksController : Controller
    {
        private readonly ParkingContext _context;
        private CacheProvider _cache;
        private const string modelName = "CarMarksViewModel";
        private int _pageSize = 20;

        public CarMarksController(ParkingContext context, CacheProvider cacheProvider)
        {
            _context = context;
            _cache = cacheProvider;
        }

        // GET: CarMarks
        public async Task<IActionResult> Index(string? CarMarkName, string FieldName, string OldFieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            CarMarkName = CookieProccesor.GetSetValue("CarMarkName", CarMarkName ?? "", first, Request, Response);

            var viewModel = _cache.GetItem<CarMarksViewModel>(modelName);

            if (viewModel != null && viewModel.CarMarkName == CarMarkName && ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName) &&
                viewModel.PageViewModel.PageNumber == page)
            {
                return View(viewModel);
            }

            var context = Search(_context.CarMarks, CarMarkName);

            var count = context.ToList().Count();

            var sortModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            context = Sort(context, sortModel).Skip((page - 1) * _pageSize).Take(_pageSize);

            viewModel = new CarMarksViewModel()
            {
                CarMarkName = CarMarkName,
                CarMarks = context,
                SortViewModel = sortModel,
                PageViewModel = new PageViewModel(count, page, _pageSize)
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: CarMarks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CarMarks == null)
            {
                return NotFound();
            }

            var carMark = await _context.CarMarks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carMark == null)
            {
                return NotFound();
            }

            return View(carMark);
        }

        // GET: CarMarks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CarMarks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] CarMark carMark)
        {
            if (ModelState.IsValid)
            {
                _context.Add(carMark);
                await _context.SaveChangesAsync();
                _cache.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(carMark);
        }

        // GET: CarMarks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CarMarks == null)
            {
                return NotFound();
            }

            var carMark = await _context.CarMarks.FindAsync(id);
            if (carMark == null)
            {
                return NotFound();
            }
            return View(carMark);
        }

        // POST: CarMarks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] CarMark carMark)
        {
            if (id != carMark.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carMark);
                    _cache.SetItem(null, modelName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarMarkExists(carMark.Id))
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
            return View(carMark);
        }

        // GET: CarMarks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CarMarks == null)
            {
                return NotFound();
            }

            var carMark = await _context.CarMarks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (carMark == null)
            {
                return NotFound();
            }

            return View(carMark);
        }

        // POST: CarMarks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CarMarks == null)
            {
                return Problem("Entity set 'ParkingContext.CarMarks'  is null.");
            }
            var carMark = await _context.CarMarks.FindAsync(id);
            if (carMark != null)
            {
                _context.CarMarks.Remove(carMark);
                _cache.SetItem(null, modelName);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarMarkExists(int id)
        {
          return _context.CarMarks.Any(e => e.Id == id);
        }

        private IEnumerable<CarMark> Search(IQueryable<CarMark> carkMarks, string CarMarkName)
        {
            return carkMarks.Where(c => c.Name.Contains(CarMarkName));
        }

        private List<CarMark> Sort(IEnumerable<CarMark> carMarks, SortViewModel sortViewModel)
        {
            Func<CarMark, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "CarMarkName":
                    func = c => c.Name;
                    break;
                default:
                    func = c => c.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    carMarks = carMarks.OrderBy(func);
                    break;
                case SortState.Descending:
                    carMarks = carMarks.OrderByDescending(func);
                    break;
            }

            return carMarks.ToList();
        }
    }
}
