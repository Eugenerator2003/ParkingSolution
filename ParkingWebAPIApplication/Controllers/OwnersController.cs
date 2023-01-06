using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Parking.Domain.Models;
using WebParking.Application;

namespace ParkingWebAPIApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : ControllerBase
    {
        private ParkingContext _context;

        public OwnersController(ParkingContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Owner> Get()
        {
            return _context.Owners.AsEnumerable();
        }
    }
}
