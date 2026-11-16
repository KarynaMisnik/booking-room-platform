
using System;

namespace backend.Models;

public class Booking
{
    public int Id { get; set; }

    public int RoomId { get; set; }  // Foreign key, ID of the room associated with the booking

    public Room Room { get; set; } = null!; // Navigation property

    public string GuestName { get; set; } = "";

    public string GuestEmail { get; set; } = "";

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
