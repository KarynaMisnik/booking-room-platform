using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using System.Security.Cryptography;
using System.Text;
using backend.DTOs;

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
    public async Task<BookingResponse> CreateBookingAsync(Booking booking)
    {
        if (string.IsNullOrWhiteSpace(booking.GuestName))
        {
            throw new ArgumentException("Guest name is required.");
        }
        if (!_validator.AreDatesValid(booking.CheckIn, booking.CheckOut))
        {
            throw new ArgumentException("Check-out must be after check-in.");
        }

        if (!await RoomExistsAsync(booking.RoomId))
        {
            throw new KeyNotFoundException("Room not found.");
        }

        bool hasOverlap = await _db.Bookings.AnyAsync(existing =>
            existing.RoomId == booking.RoomId &&
            existing.Status != "Cancelled" &&
            booking.CheckIn < existing.CheckOut &&
            booking.CheckOut > existing.CheckIn
        );

        if (hasOverlap)
        {
            throw new InvalidOperationException(
                "This room is already booked for the selected dates.");
        }

        string managementToken = GenerateManagementToken();

        booking.ManagementTokenHash = HashToken(managementToken);

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return new BookingResponse
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            GuestName = booking.GuestName,
            GuestEmail = booking.GuestEmail,
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut,
            Status = booking.Status,
            ManagementToken = managementToken
        };
    }
    // generates private token (booking management)
    private static string GenerateManagementToken() // Generates a cryptographically secure random token.
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
    }

    private static string HashToken(string token)
    {
        return Convert.ToHexString(
            SHA256.HashData(Encoding.UTF8.GetBytes(token))
        // Converts the token into a SHA-256 hash
        );
    }

    // cancel booking
    public async Task CancelBookingAsync(string managementToken)
    {
        if (string.IsNullOrWhiteSpace(managementToken))
        {
            throw new ArgumentException("Management token is required.");
        }

        string tokenHash = HashToken(managementToken);

        var booking = await _db.Bookings.FirstOrDefaultAsync(
            b => b.ManagementTokenHash == tokenHash);

        if (booking == null)
        {
            throw new KeyNotFoundException("Booking not found.");
        }

        if (booking.Status == "Cancelled")
        {
            throw new InvalidOperationException("Booking is already cancelled.");
        }

        booking.Status = "Cancelled";

        await _db.SaveChangesAsync();
    }
}