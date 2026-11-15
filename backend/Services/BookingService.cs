using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Services;

public class BookingService
{
    private readonly AppDbContext _db;
    private readonly BookingValidator _validator;
    public BookingService(AppDbContext db, BookingValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<bool> RoomExistsAsync(int roomId)
    {
        return await _db.Rooms.AnyAsync(room => room.Id == roomId);
    }
    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        if (!_validator.AreDatesValid(booking.CheckIn, booking.CheckOut))
        {
            throw new ArgumentException("Check-out must be after check-in.");
        }

        if (!await RoomExistsAsync(booking.RoomId))
        {
            throw new KeyNotFoundException("Room not found.");
        }

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return booking;
    }
}