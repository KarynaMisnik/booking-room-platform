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
    }
}