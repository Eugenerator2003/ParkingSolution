using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Parking.Domain.Models;
using WebParking.Application;
using WebParking.Managers;
using WebParking.ViewModels;

namespace WebParking.Controllers
{
    [Authorize()]
    public class EmploeyeesController : Controller
    {
        private readonly ParkingContext _context;
        private const int _pageSize = 20;
        private CacheProvider _cache;
        private const string modelName = "EmploeyyesViewModel";
        private EmployeeManager _manager;
        private ParkingAccountant _accountant;

        public EmploeyeesController(ParkingContext context, CacheProvider cacheProvider, EmployeeManager employeeManager, ParkingAccountant accountant)
        {
            _context = context;
            _cache = cacheProvider;
            _manager = employeeManager;
            _accountant = accountant;
        }

        // GET: Emploeyees
        public async Task<IActionResult> Index(string Fullname, string FieldName, string OldFieldName, SortState SortOrder, bool first=false, int page = 1)
        {
            Fullname = CookieProccesor.GetSetValue("EmploeyyeFullname", Fullname ?? "", first, Request, Response);

            var viewModel = _cache.GetItem<EmploeyeesViewModel>(modelName);

            if (viewModel != null && viewModel.Fullname == Fullname && viewModel.PageViewModel.PageNumber == page &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName))
            {
                return View(viewModel);
            }

            var context = Search(_context.Emploeyees, Fullname);

            var count = context.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            var shiftsCount = _manager.GetWorkShiftsCountWithIdThisMonth(_context.WorkShifts);

            var salaries = _accountant.GetEmployeesSalary(context, DateTime.Now, _context.WorkShifts.Include(s => s.Employee),
                                                          _context.ParkingRecords.Include(r => r.Employee), _context.PaymentTariffs);

            var employeeContext = from e in context
                                  join shift in shiftsCount
                                  on e.Id equals shift.Item1 into j1
                                  from j1i in j1.DefaultIfEmpty()
                                  join salary in salaries
                                  on j1i.Item1 equals salary.Item1 into j2
                                  from j2i in j2.DefaultIfEmpty()
                                  select (e, j1i.Item2, j2i.Item2);

            employeeContext = Sort(employeeContext, sortViewModel);

            viewModel = new EmploeyeesViewModel()
            {
                Emploeyees = employeeContext,
                Fullname = Fullname,
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize)
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: Emploeyees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Emploeyees == null)
            {
                return NotFound();
            }

            var emploeyee = await _context.Emploeyees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emploeyee == null)
            {
                return NotFound();
            }

            return View(emploeyee);
        }

        // GET: Emploeyees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Emploeyees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname")] Emploeyee emploeyee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(emploeyee);
                _context.SaveChangesAsync();
                _cache.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            return View(emploeyee);
        }

        // GET: Emploeyees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Emploeyees == null)
            {
                return NotFound();
            }

            var emploeyee = await _context.Emploeyees.FindAsync(id);
            if (emploeyee == null)
            {
                return NotFound();
            }
            return View(emploeyee);
        }

        // POST: Emploeyees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname")] Emploeyee emploeyee)
        {
            if (id != emploeyee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emploeyee);
                    _cache.SetItem(null, modelName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmploeyeeExists(emploeyee.Id))
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
            return View(emploeyee);
        }

        // GET: Emploeyees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Emploeyees == null)
            {
                return NotFound();
            }

            var emploeyee = await _context.Emploeyees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emploeyee == null)
            {
                return NotFound();
            }

            return View(emploeyee);
        }

        // POST: Emploeyees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Emploeyees == null)
            {
                return Problem("Entity set 'ParkingContext.Emploeyees'  is null.");
            }
            var emploeyee = await _context.Emploeyees.FindAsync(id);
            if (emploeyee != null)
            {
                _context.Emploeyees.Remove(emploeyee);
                _cache.SetItem(null, modelName);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmploeyeeExists(int id)
        {
          return _context.Emploeyees.Any(e => e.Id == id);
        }

        private IEnumerable<Emploeyee> Search(IQueryable<Emploeyee> emploeyees, string fullname)
        {
            return emploeyees.Where(e => e.Fullname.Contains(fullname));
        }

        private List<(Emploeyee, int, decimal)> Sort(IEnumerable<(Emploeyee, int, decimal)> emploeyees, SortViewModel sortViewModel)
        {
            Func<(Emploeyee, int, decimal), object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "Fullname":
                    func = e => e.Item1.Fullname;
                    break;
                case "Count":
                    func = e => e.Item2;
                    break;
                case "Salary":
                    func = e => e.Item3;
                    break;
                default:
                    func = e => e.Item1.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    emploeyees = emploeyees.OrderBy(func);
                    break;
                case SortState.Descending:
                    emploeyees = emploeyees.OrderByDescending(func);
                    break;
            }

            return emploeyees.ToList();
        }
    }
}
