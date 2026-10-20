
using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Room>> GetRooms()
    {
        var rooms = new List<Room>
        {
            new Room { Id = 1, Name = "Standard Room", Price = 89.00m, Capacity = 2 },
            new Room { Id = 2, Name = "Deluxe Room", Price = 129.00m, Capacity = 3 }
        };

        return Ok(rooms);
    }
}
