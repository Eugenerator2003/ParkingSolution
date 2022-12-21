global using WebParking.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Parking.Domain.Models;
using WebParking.Application;
using WebParking.Managers;
using WebParking.ViewModels;

namespace WebParking.Controllers
{
    [Authorize()]
    public class WorkShiftsController : Controller
    {
        private readonly ParkingContext _context;
        private const int _pageSize = 20;
        private const string modelName = "WorkShiftsViewModel";
        private CacheProvider _cache;
        private ParkingAccountant accountant;

        public WorkShiftsController(ParkingContext context, CacheProvider cacheProvider, ParkingAccountant parkingAccountant)
        {
            _context = context;
            accountant = parkingAccountant;
            _cache = cacheProvider;
        }

        // GET: WorkShifts
        public async Task<IActionResult> Index(int? EmployeeId, bool? SearchForDate, DateTime? Date, string FieldName, string OldFieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            EmployeeId = CookieProccesor.GetSetValue<int>("EmploeyeeId", EmployeeId.HasValue ? EmployeeId.Value.ToString() : "", first, Request, Response);
            Date = CookieProccesor.GetSetValue<DateTime>("ShiftsDate", Date.HasValue ? Date.Value.ToString() : "", first, Request, Response);
            SearchForDate = CookieProccesor.GetSetValue<bool>("ShiftsSearchForDate", SearchForDate.HasValue ? SearchForDate.Value.ToString() : "", first, Request, Response);

            var viewModel = _cache.GetItem<WorkShiftsViewModel>(modelName);

            if (viewModel != null && viewModel.EmployeeId == EmployeeId &&
                viewModel.Date.GetValueOrDefault().Date == Date.GetValueOrDefault().Date && 
                viewModel.SearchForDate == SearchForDate && viewModel.PageViewModel.PageNumber == page &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName))
            {
                return View(viewModel);
            }

            var parkingContext = Search(_context.WorkShifts.Include(w => w.Employee), SearchForDate ?? false, Date, EmployeeId);

            var count = parkingContext.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            parkingContext = Sort(parkingContext, sortViewModel);

            var payments = accountant.GetPaymentForWorkShiftsId(null, parkingContext, _context.ParkingRecords, _context.PaymentTariffs);

            var context = (from shift in parkingContext
                          join payment in payments
                          on shift.Id equals payment.Item1
                          select (shift, payment.Item2)).
                          Skip((page - 1) * _pageSize).
                          Take(_pageSize);

            viewModel = new WorkShiftsViewModel()
            {
                Date = Date == default(DateTime) ? DateTime.Now : Date,
                SearchForDate = SearchForDate,
                EmployeeId = EmployeeId,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                Employees = new SelectList(EnumerableProccesor.PushDefaultToStart(_context.Emploeyees), "Id", "Fullname",
                                           EmployeeId),
                SortViewModel = sortViewModel,
                WorkShifts = context
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: WorkShifts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WorkShifts == null)
            {
                return NotFound();
            }

            var workShift = await _context.WorkShifts
                .Include(w => w.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workShift == null)
            {
                return NotFound();
            }

            return View(workShift);
        }

        // GET: WorkShifts/Create
        public IActionResult Create()
        {
            ViewData["EmploeyeeId"] = new SelectList(_context.Emploeyees, "Id", "Fullname");
            return View();
        }

        // POST: WorkShifts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartTime,EndTime,EmploeyeeId")] WorkShift workShift)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workShift);
                await _context.SaveChangesAsync();
                _cache.SetItem(null, modelName);
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmploeyeeId"] = new SelectList(_context.Emploeyees, "Id", "Fullname", workShift.EmploeyeeId);
            return View(workShift);
        }

        // GET: WorkShifts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WorkShifts == null)
            {
                return NotFound();
            }

            var workShift = await _context.WorkShifts.FindAsync(id);
            if (workShift == null)
            {
                return NotFound();
            }
            ViewData["EmploeyeeId"] = new SelectList(_context.Emploeyees, "Id", "Fullname", workShift.EmploeyeeId);
            return View(workShift);
        }

        // POST: WorkShifts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,EmploeyeeId")] WorkShift workShift)
        {
            if (id != workShift.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workShift);
                    _cache.SetItem(null, modelName);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkShiftExists(workShift.Id))
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
            ViewData["EmploeyeeId"] = new SelectList(_context.Emploeyees, "Id", "Fullname", workShift.EmploeyeeId);
            return View(workShift);
        }

        // GET: WorkShifts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WorkShifts == null)
            {
                return NotFound();
            }

            var workShift = await _context.WorkShifts
                .Include(w => w.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workShift == null)
            {
                return NotFound();
            }

            return View(workShift);
        }

        // POST: WorkShifts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WorkShifts == null)
            {
                return Problem("Entity set 'ParkingContext.WorkShifts'  is null.");
            }
            var workShift = await _context.WorkShifts.FindAsync(id);
            if (workShift != null)
            {
                _context.WorkShifts.Remove(workShift);
                _cache.SetItem(null, modelName);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WorkShiftExists(int id)
        {
          return _context.WorkShifts.Any(e => e.Id == id);
        }

        private IEnumerable<WorkShift> Search(IQueryable<WorkShift> workShifts, bool searchForDate, DateTime? date, int? employeeId)
        {
            return workShifts.Where(s => (employeeId > 0 ? s.EmploeyeeId == employeeId : true) &&
                                          (searchForDate ? s.StartTime.Value.Date == date.Value.Date : true));
        }

        private List<WorkShift> Sort(IEnumerable<WorkShift> workShifts, SortViewModel sortViewModel)
        {
            Func<WorkShift, object> func = null;

            switch (sortViewModel.FieldName)
            {
                case "Date":
                    func = s => s.StartTime.Value.Date;
                    break;
                case "EmployeeId":
                    func = s => s.Employee.Fullname;
                    break;
                default:
                    func = s => s.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    workShifts = workShifts.OrderBy(func);
                    break;
                case SortState.Descending:
                    workShifts = workShifts.OrderByDescending(func);
                    break;
            }

            return workShifts.ToList();
        }
    }
}
