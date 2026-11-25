using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.DTOs;

namespace backend.Controllers;

[ApiController]
[Route("api/admin/rooms")]
public class AdminRoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdminRoomsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
    {
        var rooms = await _context.Rooms.ToListAsync();

        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult<Room>> CreateRoom(CreateRoomDto dto)
    {
        // map DTO to entity
        var room = new Room
        {
            Name = dto.Name.Trim(),
            Price = dto.Price,
            Capacity = dto.Capacity
        };

        // register new entity
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
    }

    // The id is extracted from the URL, and :int restricts it to an integer
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Room>> GetRoom(int id)
    {
        // FindAsync() first checks whether EF Core is already tracking the entity. 
        // If not, it queries the database using the primary key
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRoom(int id, CreateRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        room.Name = dto.Name.Trim();
        room.Price = dto.Price;
        room.Capacity = dto.Capacity;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}