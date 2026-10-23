using System;

public class Booking
{
	public int Id { get; set }
	public int RoomId {  get; set; }
	public string GuestName { get; set; }
	public string GuestEmail {  get; set; }
	public DateTime CheckIn {  get; set; }
	public DateTime CheckOut { get; set; }
	public string Status { get; set; } = "Pending";
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	
}
