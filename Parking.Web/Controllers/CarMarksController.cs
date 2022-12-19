using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Parking.Application;
using Parking.Domain;

namespace Parking.Web.Controllers
{
    public class CarMarksController : Controller
    {
        private readonly ParkingContext _context;

        public CarMarksController(ParkingContext context)
        {
            _context = context;
        }

        [Authorize()]
        // GET: CarMarks
        public async Task<IActionResult> Index()
        {
              return View(await _context.CarMarks.ToListAsync());
        }

        [Authorize()]
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
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarMarkExists(int id)
        {
          return _context.CarMarks.Any(e => e.Id == id);
        }
    }
}
