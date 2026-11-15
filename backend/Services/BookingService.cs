using System;

namespace backend.Services;

public class BookingService
{
    public bool AreDatesValid(DateTime checkIn, DateTime checkOut)
    {
        return checkOut.Date > checkIn.Date;
    }
}