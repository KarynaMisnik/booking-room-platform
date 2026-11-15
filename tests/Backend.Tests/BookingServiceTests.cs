using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Backend.Tests;

public class BookingServiceTests
{
    [Fact]
    public async Task RoomExistsAsync_ExistingRoom_ReturnsTrue()
    {
        // Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Rooms.Add(new Room
        {
            Id = 1,
            Name = "Standard Room",
            Price = 80,
            Capacity = 2
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        // Act
        bool result = await service.RoomExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    // test a nonexistent room
    [Fact]
    public async Task RoomExistsAsync_NonexistentRoom_ReturnsFalse()
    {
        // Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new BookingService(db, new BookingValidator());

        // Act
        bool result = await service.RoomExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    // test successful booking creation
    [Fact]
    public async Task CreateBookingAsync_ValidBooking_SavesToDatabase()
    {
        // Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        db.Rooms.Add(new Room
        {
            Id = 1,
            Name = "Standard Room",
            Price = 80,
            Capacity = 2
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        var booking = new Booking
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "guest@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await service.CreateBookingAsync(booking);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(1, await db.Bookings.CountAsync());
    }

    // test that invalid dates prevent booking creation
    [Fact]
    public async Task CreateBookingAsync_InvalidDates_DoesNotSaveBooking()
    {
        // Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new BookingService(db, new BookingValidator());

        var booking = new Booking
        {
            RoomId = 1,
            GuestName = "Test Guest",
            GuestEmail = "guest@example.com",
            CheckIn = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(booking));

        Assert.Empty(await db.Bookings.ToListAsync());
    }

    // test booking creation with a nonexistent room: 
    // verify that CreateBookingAsync() rejects the booking and doesn't save anything
    [Fact]
    public async Task CreateBookingAsync_NonexistentRoom_DoesNotSaveBooking()
    {
        // Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var service = new BookingService(db, new BookingValidator());

        var booking = new Booking
        {
            RoomId = 999,
            GuestName = "Test Guest",
            GuestEmail = "guest@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(booking));

        Assert.Empty(await db.Bookings.ToListAsync());
    }
}