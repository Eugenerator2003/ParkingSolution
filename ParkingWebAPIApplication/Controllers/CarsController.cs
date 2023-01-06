using Arch.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Models;
using WebParking.Application;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ParkingWebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private ParkingContext _context;

        public CarsController(ParkingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> Get()
        {
            var marks = _context.CarMarks.ToList();
            var owners = _context.Owners.ToList();

            var result = _context.Cars.Include(c => c.CarMark).Include(c => c.Owner);

            return Ok(result);
        }

        // GET api/<CarsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var cars = _context.Cars.Include(c => c.CarMark).Include(c => c.Owner).ToArray();
            var result = cars.FirstOrDefault(c => c.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/<CarsController>
        [HttpPost]
        public async Task<ActionResult> Post(Car car)
        {
            if (ModelState.IsValid)
            {
                if (car != null)
                {
                    _context.Cars.Add(car);
                    await _context.SaveChangesAsync();
                    return Ok(car);
                }
            }
            return BadRequest();
        }

        // PUT api/<CarsController>/5
        [HttpPut]
        public async Task<ActionResult> Put(Car car)
        {
            if (ModelState.IsValid)
            {
                if  (car != null)
                {
                    var cars = _context.Cars.ToArray();

                    if (cars.Any(c => c.Id == car.Id))
                    {
                        _context.ChangeTracker.Clear();
                        _context.Update(car);
                        await _context.SaveChangesAsync();
                        return Ok(car);
                    }
                }
            }
            return BadRequest();
        }

        // DELETE api/<CarsController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int? id)
        {
            var cars = _context.Cars.ToArray();
            var car = cars.FirstOrDefault(c => c.Id == id);
            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
                return Ok(car);
            }
            return NotFound();

        }
    }
}
