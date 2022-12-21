global using WebParking.CacheService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebParking.Application;
using WebParking.ViewModels;
using WebParking.Managers;
using Parking.Domain.Models;

namespace WebParking.Controllers
{
    [Authorize()]
    public class OwnersController : Controller
    {
        private readonly ParkingContext _context;
        private CacheProvider _cache;
        private const int _pageSize = 20;
        private const string modelName = "OwnersViewModel";
        private OwnerManager _manager;

        public OwnersController(ParkingContext context, OwnerManager ownerManager, CacheProvider cacheProvider)
        {
            _context = context;
            _cache = cacheProvider;
            _manager = ownerManager;
        }

        // GET: Owners
        public async Task<IActionResult> Index(string Fullname, long? PhoneNumber, string FieldName, string OldFieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            Fullname = CookieProccesor.GetSetValue("OwnerFullname", Fullname ?? "", first, Request, Response);
            PhoneNumber = CookieProccesor.GetSetValue<long>("PhoneNumber", PhoneNumber.HasValue ? PhoneNumber.Value.ToString() : "", first, Request, Response);

            var viewModel = _cache.GetItem<CarOwnersViewModel>(modelName);

            if (viewModel != null && viewModel.Fullname == Fullname && viewModel.PhoneNumber == PhoneNumber &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName) &&
                viewModel.PageViewModel.PageNumber == page)
            {
                return View(viewModel);
            }

            var context = Search(_context.Owners, Fullname ?? "", PhoneNumber);

            var count = context.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            context = Sort(context, sortViewModel).Skip((page - 1) * _pageSize).Take(_pageSize);

            viewModel = new CarOwnersViewModel()
            {
                PageViewModel = new PageViewModel(count, page, _pageSize),
                SortViewModel = sortViewModel,
                Fullname = Fullname,
                PhoneNumber = PhoneNumber,
                Owners = context
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: Owners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Owners == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // GET: Owners/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Owners/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname,PhoneNumber")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(owner);
                await _context.SaveChangesAsync();
                _cache.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(owner);
        }

        // GET: Owners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Owners == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }
            return View(owner);
        }

        // POST: Owners/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,PhoneNumber")] Owner owner)
        {
            if (id != owner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    _cache.SetItem(null, modelName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerExists(owner.Id))
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
            return View(owner);
        }

        // GET: Owners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Owners == null)
            {
                return NotFound();
            }

            var owner = await _context.Owners
                .FirstOrDefaultAsync(m => m.Id == id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        // POST: Owners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Owners == null)
            {
                return Problem("Entity set 'ParkingContext.Owners'  is null.");
            }
            var owner = await _context.Owners.FindAsync(id);
            if (owner != null)
            {
                _context.Owners.Remove(owner);
                _cache.SetItem(null, modelName);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET
        public async Task<IActionResult> Regular()
        {
            IEnumerable<ParkingRecord> records = _context.ParkingRecords
                                                 .Include(r => r.Car)
                                                 .Include(r => r.Car.CarMark)
                                                 .Include(r => r.Car.Owner);

            var date = DateTime.Now.AddMonths(1);

            var owners = _manager.GetRegularOwners(records, date);

            ViewBag.Date = date;

            return View(owners);
        }

        private bool OwnerExists(int id)
        {
          return _context.Owners.Any(e => e.Id == id);
        }

        private IEnumerable<Owner> Search(IQueryable<Owner> owners, string fullname, long? number)
        {
            return owners.Where(o => o.Fullname.Contains(fullname) && (number.HasValue ? o.PhoneNumber.ToString().Contains(number.Value.ToString()): true));
        }                                                                                             

        private List<Owner> Sort(IEnumerable<Owner> owners, SortViewModel sortViewModel)
        {
            Func<Owner, object> func = null;

            switch (sortViewModel.FieldName)
            {
                case "Fullname":
                    func = o => o.Fullname;
                    break;
                case "PhoneNumber":
                    func = o => o.PhoneNumber;
                    break;
                default:
                    func = o => o.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    owners = owners.OrderBy(func);
                    break;
                case SortState.Descending:
                    owners = owners.OrderByDescending(func);
                    break;
            }

            return owners.ToList();
        }
    }
}
