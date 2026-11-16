using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        var rooms = new List<Room>
        {
            new Room { Name = "Standard Room", Price = 89.00m, Capacity = 2 },
            new Room { Name = "Deluxe Room", Price = 129.00m, Capacity = 3 },
            new Room { Name = "Family Room", Price = 159.00m, Capacity = 4 },
            new Room { Name = "Superior Room", Price = 189.00m, Capacity = 2 },
            new Room { Name = "Luxury Suite", Price = 299.00m, Capacity = 2 }
        };

        foreach (var room in rooms)
        {
            bool exists = await db.Rooms.AnyAsync(r => r.Name == room.Name);

            if (!exists)
            {
                db.Rooms.Add(room);
            }
        }

        await db.SaveChangesAsync();

        // Seed sample bookings only when the database has no bookings
        if (!await db.Bookings.AnyAsync())
        {
            var today = DateTime.UtcNow.Date;

            var standardRoom = await db.Rooms.SingleAsync(r => r.Name == "Standard Room");
            var deluxeRoom = await db.Rooms.SingleAsync(r => r.Name == "Deluxe Room");
            var familyRoom = await db.Rooms.SingleAsync(r => r.Name == "Family Room");

            var bookings = new List<Booking>
    {
        new Booking
        {
            RoomId = standardRoom.Id,
            GuestName = "Alice Johnson",
            GuestEmail = "alice@example.com",
            CheckIn = today.AddDays(2),
            CheckOut = today.AddDays(5),
            Status = "Confirmed"
        },
        new Booking
        {
            RoomId = deluxeRoom.Id,
            GuestName = "Michael Brown",
            GuestEmail = "michael@example.com",
            CheckIn = today.AddDays(4),
            CheckOut = today.AddDays(7),
            Status = "Pending"
        },
        new Booking
        {
            RoomId = familyRoom.Id,
            GuestName = "Emma Wilson",
            GuestEmail = "emma@example.com",
            CheckIn = today.AddDays(8),
            CheckOut = today.AddDays(12),
            Status = "Cancelled"
        }
    };

            db.Bookings.AddRange(bookings);
            await db.SaveChangesAsync();
        }
    }
}