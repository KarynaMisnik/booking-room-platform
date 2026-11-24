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

    // prevent overlapping booking
    [Fact]
    public async Task CreateBookingAsync_OverlappingDates_RejectsBooking()
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

        db.Bookings.Add(new Booking
        {
            RoomId = 1,
            GuestName = "Alice",
            GuestEmail = "alice@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = "Confirmed"
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        var newBooking = new Booking
        {
            RoomId = 1,
            GuestName = "Bob",
            GuestEmail = "bob@example.com",
            CheckIn = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 17, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateBookingAsync(newBooking));

        Assert.Equal(1, await db.Bookings.CountAsync());
    }

    // test that adjacent bookings are allowed
    [Fact]
    public async Task CreateBookingAsync_AdjacentDates_AllowsBooking()
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

        db.Bookings.Add(new Booking
        {
            RoomId = 1,
            GuestName = "Alice",
            GuestEmail = "alice@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = "Confirmed"
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        var newBooking = new Booking
        {
            RoomId = 1,
            GuestName = "Bob",
            GuestEmail = "bob@example.com",
            CheckIn = new DateTime(2026, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 17, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await service.CreateBookingAsync(newBooking);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(2, await db.Bookings.CountAsync());
    }

    // verify that two different rooms can be booked for the same dates
    [Fact]
    public async Task CreateBookingAsync_DifferentRooms_AllowsBooking()
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

        db.Bookings.Add(new Booking
        {
            RoomId = 1,
            GuestName = "Alice",
            GuestEmail = "alice@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = "Confirmed"
        });

        db.Rooms.Add(new Room
        {
            Id = 2,
            Name = "Deluxe Room",
            Price = 120,
            Capacity = 2
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        var newBooking = new Booking
        {
            RoomId = 2,
            GuestName = "Bob",
            GuestEmail = "bob@example.com",
            CheckIn = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 17, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await service.CreateBookingAsync(newBooking);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(2, await db.Bookings.CountAsync());
    }

    // test cancelled bookings: cancelled booking should not block another guest from booking the same room for the same dates
    [Fact]
    public async Task CreateBookingAsync_CancelledBooking_AllowsBooking()
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

        db.Bookings.Add(new Booking
        {
            RoomId = 1,
            GuestName = "Alice",
            GuestEmail = "alice@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 15, 0, 0, 0, DateTimeKind.Utc),
            Status = "Cancelled"
        });

        await db.SaveChangesAsync();

        var service = new BookingService(db, new BookingValidator());

        var newBooking = new Booking
        {
            RoomId = 1,
            GuestName = "Bob",
            GuestEmail = "bob@example.com",
            CheckIn = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 17, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act
        var result = await service.CreateBookingAsync(newBooking);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal(2, await db.Bookings.CountAsync());
    }

    // guest name validation
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateBookingAsync_InvalidGuestName_DoesNotSaveBooking(
    string? guestName)
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
            GuestName = guestName!,
            GuestEmail = "guest@example.com",
            CheckIn = new DateTime(2026, 12, 10, 0, 0, 0, DateTimeKind.Utc),
            CheckOut = new DateTime(2026, 12, 12, 0, 0, 0, DateTimeKind.Utc)
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBookingAsync(booking));

        Assert.Equal("Guest name is required.", exception.Message);
        Assert.Empty(await db.Bookings.ToListAsync());
    }

}