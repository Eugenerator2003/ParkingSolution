using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebParking.Application;
using WebParking.Domain;
using WebParking.ViewModels;

namespace WebParking.Controllers
{
    public class PaymentTariffsController : Controller
    {
        private readonly ParkingContext _context;
        private const string modelName = "PaymentTariffsViewModel";
        private const int _pageSize = 20;
        private CacheProvider _cache;
        
        public PaymentTariffsController(ParkingContext context, CacheProvider cacheProvider)
        {
            _context = context;
            _cache = cacheProvider;
        }

        // GET: PaymentTariffs
        public async Task<IActionResult> Index(int? ParkingTypeId, int? DaysCount, decimal? Payment, string FieldName, string OldFieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            ParkingTypeId = CookieProccesor.GetSetValue<int>("ParkingTypeId", ParkingTypeId.HasValue ? ParkingTypeId.Value.ToString() : "", first, Request, Response);
            DaysCount = CookieProccesor.GetSetValue<int>("DaysCount", DaysCount.HasValue ? DaysCount.Value.ToString() : "", first, Request, Response);
            Payment = CookieProccesor.GetSetValue<decimal>("Payment", Payment.HasValue ? Payment.Value.ToString() : "", first, Request, Response);

            var viewModel = _cache.GetItem<PaymentTariffsViewModel>(modelName);

            if (viewModel != null && viewModel.ParkingTypeId == ParkingTypeId && viewModel.DaysCount == DaysCount && 
                viewModel.Payment == Payment && viewModel.PageViewModel.PageNumber == page &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName))
            {
                return View(viewModel);
            }

            var parkingContext = Search(_context.PaymentTariffs.Include(p => p.ParkingType), Payment, DaysCount, ParkingTypeId);

            var count = parkingContext.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            parkingContext = Sort(parkingContext, sortViewModel).Skip((page - 1) * _pageSize).Take(_pageSize);

            viewModel = new PaymentTariffsViewModel()
            {
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                DaysCount = DaysCount,
                ParkingTypeId = ParkingTypeId,
                Payment = Payment,
                PaymentTariffs = parkingContext,
                ParkingTypes = new SelectList(PushDefault(_context.ParkingTypes), "Id", "Name", ParkingTypeId),
            };

            return View(viewModel);
        }

        // GET: PaymentTariffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.PaymentTariffs == null)
            {
                return NotFound();
            }

            var paymentTariff = await _context.PaymentTariffs
                .Include(p => p.ParkingType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentTariff == null)
            {
                return NotFound();
            }

            return View(paymentTariff);
        }

        // GET: PaymentTariffs/Create
        public IActionResult Create()
        {
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Name");
            return View();
        }

        // POST: PaymentTariffs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParkingTypeId,DaysCount,Payment")] PaymentTariff paymentTariff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentTariff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Name", paymentTariff.ParkingTypeId);
            return View(paymentTariff);
        }

        // GET: PaymentTariffs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.PaymentTariffs == null)
            {
                return NotFound();
            }

            var paymentTariff = await _context.PaymentTariffs.FindAsync(id);
            if (paymentTariff == null)
            {
                return NotFound();
            }
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Name", paymentTariff.ParkingTypeId);
            return View(paymentTariff);
        }

        // POST: PaymentTariffs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ParkingTypeId,DaysCount,Payment")] PaymentTariff paymentTariff)
        {
            if (id != paymentTariff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentTariff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentTariffExists(paymentTariff.Id))
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
            ViewData["ParkingTypeId"] = new SelectList(_context.ParkingTypes, "Id", "Name", paymentTariff.ParkingTypeId);
            return View(paymentTariff);
        }

        // GET: PaymentTariffs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.PaymentTariffs == null)
            {
                return NotFound();
            }

            var paymentTariff = await _context.PaymentTariffs
                .Include(p => p.ParkingType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentTariff == null)
            {
                return NotFound();
            }

            return View(paymentTariff);
        }

        // POST: PaymentTariffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.PaymentTariffs == null)
            {
                return Problem("Entity set 'ParkingContext.PaymentTariffs'  is null.");
            }
            var paymentTariff = await _context.PaymentTariffs.FindAsync(id);
            if (paymentTariff != null)
            {
                _context.PaymentTariffs.Remove(paymentTariff);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentTariffExists(int id)
        {
          return _context.PaymentTariffs.Any(e => e.Id == id);
        }

        private IEnumerable<PaymentTariff> Search(IQueryable<PaymentTariff> paymentTariffs, decimal? payment, int? daysCount, int? parkingTypeId)
        {
            return paymentTariffs.Where(p => (p.Payment >= payment) &&
                                             (daysCount > 0 ? p.DaysCount == daysCount : true) &&
                                             (parkingTypeId > 0 ? p.ParkingTypeId == parkingTypeId : true));
        }

        private List<PaymentTariff> Sort(IEnumerable<PaymentTariff> paymentTariffs, SortViewModel sortViewModel)
        {
            Func<PaymentTariff, object> func = null;

            switch(sortViewModel.FieldName)
            {
                case "DaysCount":
                    func = p => p.DaysCount;
                    break;
                case "ParkingTypeId":
                    func = p => p.ParkingTypeId;
                    break;
                case "Payment":
                    func = p => p.Payment;
                    break;
                default:
                    func = p => p.Id;
                    break;
            }

            switch (sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    paymentTariffs = paymentTariffs.OrderBy(func);
                    break;
                case SortState.Descending:
                    paymentTariffs = paymentTariffs.OrderByDescending(func);
                    break;
            }

            return paymentTariffs.ToList();
        }

        public List<T> PushDefault<T>(IEnumerable<T> entities) where T : new()
        {
            var list = entities.ToList();
            list.Insert(0, new T { });
            return list;
        }

    }
}
