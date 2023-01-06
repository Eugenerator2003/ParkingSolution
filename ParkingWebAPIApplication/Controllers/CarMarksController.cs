using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Models;
using WebParking.Application;

namespace ParkingWebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarMarksController : ControllerBase
    {
        private ParkingContext _context;

        public CarMarksController(ParkingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<CarMark> Get()
        {
            return _context.CarMarks.AsEnumerable();
        }
    }
}
