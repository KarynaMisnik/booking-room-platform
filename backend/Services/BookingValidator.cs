namespace backend.Services;

public class BookingValidator
{
    public bool AreDatesValid(DateTime checkIn, DateTime checkOut)
    {
        return checkOut.Date < checkIn.Date;
    }
}