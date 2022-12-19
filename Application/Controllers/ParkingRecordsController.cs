using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebParking.Application;
using WebParking.Domain;
using WebParking.Managers;
using WebParking.ViewModels;

namespace WebParking.Controllers
{
    public class ParkingRecordsController : Controller
    {
        private readonly ParkingContext _context;
        private const int _pageSize = 20;
        private const string modelName = "ParkingRecordsViewModel";
        private CacheProvider _cache;
        private ParkingAccountant _accountant;

        public ParkingRecordsController(ParkingContext context, CacheProvider cacheProvider, ParkingAccountant parkingAccountant)
        {
            _context = context;
            _cache = cacheProvider;
            _accountant = parkingAccountant;
        }

        // GET: ParkingRecords
        public async Task<IActionResult> Index(int? ParkingTypeId, bool? SearchForEntryDate, bool? SearchForDepatureDate, DateTime? EntryDate, DateTime? DepartureDate, int? OwnerId, int? CarId, string FieldName, string OldFieldName, SortState SortOrder, bool first = false, int page = 1)
        {
            EntryDate = CookieProccesor.GetSetValue<DateTime>("EntryDate", EntryDate.HasValue ? EntryDate.Value.ToString() : "", first, Request, Response);
            DepartureDate = CookieProccesor.GetSetValue<DateTime>("DepatureDate", DepartureDate.HasValue ? DepartureDate.Value.ToString() : "", first, Request, Response);
            SearchForEntryDate = CookieProccesor.GetSetValue<bool>("SearchForEntryDate", SearchForEntryDate.ToString(), first, Request, Response);
            SearchForDepatureDate = CookieProccesor.GetSetValue<bool>("SearchForDepatureDate", SearchForDepatureDate.ToString(), first, Request, Response);
            CarId = CookieProccesor.GetSetValue<int>("RecordCarId", CarId.HasValue ? CarId.Value.ToString() : "", first, Request, Response);
            OwnerId = CookieProccesor.GetSetValue<int>("RecordCarId", OwnerId.HasValue ? OwnerId.Value.ToString() : "", first, Request, Response);
            ParkingTypeId = CookieProccesor.GetSetValue<int>("RecordParkingTypeId", ParkingTypeId.HasValue ? ParkingTypeId.Value.ToString() : "", first, Request, Response);

            var viewModel = _cache.GetItem<ParkingRecordsViewModel>(modelName);

            if (viewModel != null && viewModel.CarId == CarId && viewModel.OwnerId == OwnerId &&
                viewModel.SearchForDepatureDate == viewModel.SearchForDepatureDate &&
                viewModel.SearchForEntryDate == viewModel.SearchForEntryDate &&
                viewModel.DepartureDate == DepartureDate && viewModel.EntryDate == EntryDate &&
                viewModel.PageViewModel.PageNumber == page &&
                viewModel.ParkingTypeId == ParkingTypeId.Value &&
                ViewModelComparsion.Compare(viewModel.SortViewModel, SortOrder, FieldName))
            {
                return View(viewModel);
            }

            var parkingContext = _context.ParkingRecords.Include(p => p.Car).
                                                        Include(p => p.Car.Owner).
                                                        Include(p => p.Car.CarMark).
                                                        Include(p => p.PaymentTariffId).
                                                        Include(p => p.PaymentTariffId.ParkingType).
                                                        Include(p => p.Employee);

            var context = Search(parkingContext, SearchForEntryDate ?? false, SearchForDepatureDate ?? false, EntryDate, DepartureDate,
                                 CarId ?? 0, OwnerId ?? 0, ParkingTypeId ?? 0);

            var count = context.Count();

            var sortViewModel = new SortViewModel(SortOrder, FieldName, OldFieldName);

            context = Sort(context, sortViewModel).Skip((page - 1) * _pageSize).Take(_pageSize).ToList();

            var list = new List<(ParkingRecord, decimal)>();
            var tariffs = _context.PaymentTariffs;
            foreach(var record in context)
            {
                list.Add((record, _accountant.GetPayment(record, tariffs)));
            }

            viewModel = new ParkingRecordsViewModel()
            {
                ParkingRecords = list,
                SearchForDepatureDate = SearchForDepatureDate ?? false,
                SearchForEntryDate = SearchForEntryDate ?? false,
                EntryDate = EntryDate == default(DateTime) ? DateTime.Now : EntryDate,
                DepartureDate = DepartureDate == default(DateTime) ? DateTime.Now : DepartureDate,
                CarId = CarId ?? 0,
                OwnerId = OwnerId ?? 0,
                ParkingTypeId = ParkingTypeId ?? 0,
                SortViewModel = sortViewModel,
                PageViewModel = new PageViewModel(count, page, _pageSize),
                Cars = new SelectList(EnumerableProccesor.PushDefaultToStart(_context.Cars), "Id", "Number", CarId),
                Onwers = new SelectList(EnumerableProccesor.PushDefaultToStart(_context.Owners), "Id", "Fullname", OwnerId),
                ParkingTypes = new SelectList(EnumerableProccesor.PushDefaultToStart(_context.ParkingTypes), "Id", "Name", ParkingTypeId)
            };

            _cache.SetItem(viewModel, modelName);

            return View(viewModel);
        }

        // GET: ParkingRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ParkingRecords == null)
            {
                return NotFound();
            }

            var parkingRecord = await _context.ParkingRecords
                .Include(p => p.Car)
                .Include(p => p.Employee)
                .Include(p => p.PaymentTariffId.ParkingType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingRecord == null)
            {
                return NotFound();
            }

            return View(parkingRecord);
        }

        // GET: ParkingRecords/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Number");
            ViewData["ParkingType"] = new SelectList(_context.ParkingTypes, "Id", "Name");
            //ViewData["EmployeeId"] = new SelectList(_context.Emploeyees, "Id", "Fullname");
            //ViewData["PaymentTariffIdId"] = new SelectList(_context.PaymentTariffs, "Id", "Id");
            return View();
        }

        // POST: ParkingRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? CarId, DateTime? DepatureTime, DateTime? EntryTime, int? PaymentTariffIdId)
        {
            //[Bind("Id,CarId,EntryTime,DepartureTime,PaymentTariffIdId,EmployeeId")]

            //if (ModelState.IsValid)
            //{
            //    _context.Add(parkingRecord);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction(nameof(Index));
            //}

            var tariffId = PaymentTariffIdId.Value > 1 ? 1 : 4;

            var parkingRecord = new ParkingRecord()
            {
                CarId = CarId,
                DepartureTime = DepatureTime,
                EntryTime = EntryTime,
                PaymentTariffIdId = tariffId
            };

            _context.Add(parkingRecord);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Number", parkingRecord.CarId);
            ViewData["ParkingType"] = new SelectList(_context.ParkingTypes, "Id", "Name", parkingRecord.PaymentTariffId.ParkingTypeId);
            //ViewData["EmployeeId"] = new SelectList(_context.Emploeyees, "Id", "Id", parkingRecord.EmployeeId);
            //ViewData["PaymentTariffIdId"] = new SelectList(_context.PaymentTariffs, "Id", "Id", parkingRecord.PaymentTariffIdId);
            return View(parkingRecord);
        }

        // GET: ParkingRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ParkingRecords == null)
            {
                return NotFound();
            }

            var parkingRecord = await _context.ParkingRecords.FindAsync(id);
            if (parkingRecord == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Number", parkingRecord.CarId);
            ViewData["ParkingType"] = new SelectList(_context.ParkingTypes, "Id", "Name", parkingRecord.PaymentTariffId.ParkingTypeId);
            //ViewData["EmployeeId"] = new SelectList(_context.Emploeyees, "Id", "Id", parkingRecord.EmployeeId);
            //ViewData["PaymentTariffIdId"] = new SelectList(_context.PaymentTariffs, "Id", "Id", parkingRecord.PaymentTariffIdId);
            return View(parkingRecord);
        }

        // POST: ParkingRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? CarId, DateTime? DepatureTime, DateTime? EntryTime, int? PaymentTariffIdId)
        {
            var tariffId = PaymentTariffIdId.Value > 1 ? 1 : 4;

            var parkingRecord = new ParkingRecord()
            {
                Id = id,
                CarId = CarId,
                DepartureTime = DepatureTime,
                EntryTime = EntryTime,
                PaymentTariffIdId = tariffId
            };

            if (id != parkingRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(parkingRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParkingRecordExists(parkingRecord.Id))
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
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "Number", parkingRecord.CarId);
            ViewData["ParkingType"] = new SelectList(_context.ParkingTypes, "Id", "Name", parkingRecord.PaymentTariffId.ParkingTypeId);
            //ViewData["EmployeeId"] = new SelectList(_context.Emploeyees, "Id", "Id", parkingRecord.EmployeeId);
            //ViewData["PaymentTariffIdId"] = new SelectList(_context.PaymentTariffs, "Id", "Id", parkingRecord.PaymentTariffIdId);
            return View(parkingRecord);
        }

        // GET: ParkingRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ParkingRecords == null)
            {
                return NotFound();
            }

            var parkingRecord = await _context.ParkingRecords
                .Include(p => p.Car)
                .Include(p => p.Employee)
                .Include(p => p.PaymentTariffId.ParkingType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (parkingRecord == null)
            {
                return NotFound();
            }

            return View(parkingRecord);
        }

        // POST: ParkingRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ParkingRecords == null)
            {
                return Problem("Entity set 'ParkingContext.ParkingRecords'  is null.");
            }
            var parkingRecord = await _context.ParkingRecords.FindAsync(id);
            if (parkingRecord != null)
            {
                _context.ParkingRecords.Remove(parkingRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Salary(DateTime? Date)
        {
            decimal? salary = null;
            decimal? earning = null;
            if (Date is not null)
            {
                salary = _accountant.GetSalary(Date, _context.ParkingRecords, _context.PaymentTariffs);
                earning = _accountant.GetEarnings(Date, _context.ParkingRecords, _context.PaymentTariffs);
            }

            var viewModel = new EarningBillViewModel()
            {
                Salary = salary,
                Earning = earning,
                Date = Date ?? DateTime.Now,
            };

            return View(viewModel);
        }

        private bool ParkingRecordExists(int id)
        {
            return _context.ParkingRecords.Any(e => e.Id == id);
        }

        private IEnumerable<ParkingRecord> Search(IQueryable<ParkingRecord> parkingRecords, bool SearchForEntryDate,
                                                 bool SearchForDepatureDate, DateTime? EntryDate, DateTime? DepatureDate,
                                                 int CarId, int OwnerId, int parkingTypeId)
        {
            return parkingRecords.Where(r => (CarId > 0 ? r.CarId == CarId : true) &&
                                             (OwnerId > 0 ? r.Car.OwnerId == OwnerId : true) &&
                                             (SearchForDepatureDate ? r.DepartureTime == DepatureDate : true) &&
                                             (SearchForEntryDate ? r.EntryTime == EntryDate : true) &&
                                             (parkingTypeId > 0 ? r.PaymentTariffId.ParkingTypeId == parkingTypeId : true));
        }

        private IEnumerable<ParkingRecord> Sort(IEnumerable<ParkingRecord> parkingRecords, SortViewModel sortViewModel)
        {
            Func<ParkingRecord, object> func = null;

            switch (sortViewModel.FieldName)
            {
                case "OwnerId":
                    func = r => r.Car.Owner.Fullname;
                    break;
                case "OwnerNumber":
                    func = r => r.Car.Owner.PhoneNumber;
                    break;
                case "CarId":
                    func = r => r.Car.Number;
                    break;
                case "CarMark":
                    func = r => r.Car.CarMark.Name;
                    break;
                case "EntryTime":
                    func = r => r.EntryTime;
                    break;
                case "DepartureTime":
                    func = r => r.DepartureTime;
                    break;
                default:
                    func = r => r.Id;
                    break;
            }

            switch(sortViewModel.CurrentState)
            {
                case SortState.Ascending:
                    parkingRecords = parkingRecords.OrderBy(func);
                    break;
                case SortState.Descending:
                    parkingRecords = parkingRecords.OrderByDescending(func);
                    break;
            }

            return parkingRecords.ToList();
        }
    }
}
