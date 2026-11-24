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
    [Theory] // allows the same test method to run with different input values
    [InlineData("")] // supplies an input value for each test execution
    [InlineData("   ")] // allows the parameter to contain null
    [InlineData(null)] // suppresses the nullable warning because we intentionally test invalid input

    // creates a booking and returns its management token.
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

    // cancel booking
    [Fact]

    // uses created token to cancel the booking
    public async Task CancelBookingAsync_ValidToken_UpdatesBookingStatus()
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

        var createdBooking = await service.CreateBookingAsync(booking);

        // Act
        await service.CancelBookingAsync(createdBooking.ManagementToken);

        // Assert
        var savedBooking = await db.Bookings
            .AsNoTracking()
            // reads the saved data from the db without reusing the tracked entity
            .SingleAsync();

        Assert.Equal("Cancelled", savedBooking.Status);
        Assert.Equal(1, await db.Bookings.CountAsync());
    }

    // verify that only user with correct token can cancel
    [Fact]


    // An incorrect token cannot cancel an existing reservation
    //The original booking status remains Pending
    //The booking record remains in the database
    public async Task CancelBookingAsync_InvalidToken_DoesNotCancelBooking()
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

        var createdBooking = await service.CreateBookingAsync(booking);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CancelBookingAsync("invalid-token"));

        var savedBooking = await db.Bookings
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal("Pending", savedBooking.Status);
        Assert.Equal(1, await db.Bookings.CountAsync());
    }

    // repeated cancellation
    [Fact]

    public async Task CancelBookingAsync_AlreadyCancelled_ThrowsException()
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

        var createdBooking = await service.CreateBookingAsync(booking);

        // Cancel the booking for the first time
        await service.CancelBookingAsync(createdBooking.ManagementToken);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CancelBookingAsync(createdBooking.ManagementToken));

        Assert.Equal("Booking is already cancelled.", exception.Message);

        var savedBooking = await db.Bookings
            .AsNoTracking()
            .SingleAsync();

        Assert.Equal("Cancelled", savedBooking.Status);
    }

    // cancel room can be booked again

    [Fact]

    public async Task CancelBookingAsync_ReleasesRoomAvailability()
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

        var createdBooking = await service.CreateBookingAsync(booking);

        // Act
        await service.CancelBookingAsync(createdBooking.ManagementToken);

        var newBooking = new Booking
        {
            RoomId = 1,
            GuestName = "Another Guest",
            GuestEmail = "another@example.com",
            CheckIn = booking.CheckIn,
            CheckOut = booking.CheckOut
        };

        var result = await service.CreateBookingAsync(newBooking);

        // Assert
        Assert.True(result.Id > 0);
        Assert.Equal("Pending", result.Status);
        Assert.Equal(2, await db.Bookings.CountAsync());
    }

    // verify secure token storage (not in db)
    // The guest receives a management token
    // The database contains a token hash
    // The original token and stored value are different

    [Fact]


    public async Task CreateBookingAsync_StoresHashedManagementToken()
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

        var createdBooking = await service.CreateBookingAsync(booking);

        // Assert
        var savedBooking = await db.Bookings
            .AsNoTracking()
            .SingleAsync();

        Assert.False(string.IsNullOrWhiteSpace(
            createdBooking.ManagementToken));

        Assert.False(string.IsNullOrWhiteSpace(
            savedBooking.ManagementTokenHash));

        Assert.NotEqual(
            createdBooking.ManagementToken,
            savedBooking.ManagementTokenHash);
    }


}