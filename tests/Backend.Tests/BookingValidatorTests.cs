using backend.Services;

namespace Backend.Tests;

public class BookingValidatorTests
{
    [Fact]
    public void AreDatesValid_CheckOutAfterCheckIn_ReturnsTrue()
    {
        // Arrange
        BookingValidator validator = new BookingValidator();

        DateTime checkIn = new DateTime(2026, 12, 10);
        DateTime checkOut = new DateTime(2026, 12, 12);

        // Act
        bool result = validator.AreDatesValid(checkIn, checkOut);

        // Assert
        Assert.True(result); // only if AreDatesValid() is true
    }

    // Check-in and check-out are on the same day must be rejected
    [Fact]
    public void AreDatesValid_SameDay_ReturnsFalse()
    {
        // Arrange
        BookingValidator validator = new BookingValidator();

        DateTime checkIn = new DateTime(2026, 12, 10, 15, 0, 0);
        DateTime checkOut = new DateTime(2026, 12, 10, 18, 0, 0);

        // Act
        bool result = validator.AreDatesValid(checkIn, checkOut);

        // Assert
        Assert.False(result);
    }

    // Checks reversed dates: check-out date must be after check-in date
    [Fact]
    public void AreDatesValid_CheckOutBeforeCheckIn_ReturnsFalse()
    {
        // Arrange
        BookingValidator validator = new BookingValidator();

        DateTime checkIn = new DateTime(2026, 12, 12);
        DateTime checkOut = new DateTime(2026, 12, 10);

        // Act
        bool result = validator.AreDatesValid(checkIn, checkOut);

        // Assert
        Assert.False(result); //verifies that method rejects an invalid date range
    }
}