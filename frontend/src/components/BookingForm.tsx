import { useState, type FormEvent } from "react";

type BookingFormProps = {
  roomId: number;
};

function BookingForm({ roomId }: BookingFormProps) {
  const [guestName, setGuestName] = useState("");
  const [guestEmail, setGuestEmail] = useState("");
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [message, setMessage] = useState("");

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage("");

    const booking = {
      roomId,
      guestName,
      guestEmail,
      checkIn: `${checkIn}T15:00:00Z`,
      checkOut: `${checkOut}T11:00:00Z`,
    };

    try {
      const response = await fetch("http://localhost:5000/api/bookings", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(booking),
      });

      if (response.ok) {
        setMessage("Booking created successfully!");
      } else {
        setMessage("Booking failed. Please check your details.");
      }
    } catch (error) {
      console.error("Booking request failed:", error);
      setMessage("Cannot connect to the server. Please try again later.");
    }
  }

  return (
    <form onSubmit={handleSubmit}>
      <h3>Book Room {roomId}</h3>

      <label htmlFor={`name-${roomId}`}>Guest name:</label>
      <input
        id={`name-${roomId}`}
        type="text"
        value={guestName}
        onChange={(e) => setGuestName(e.target.value)}
        required
      />

      <label htmlFor={`email-${roomId}`}>Email:</label>
      <input
        id={`email-${roomId}`}
        type="email"
        value={guestEmail}
        onChange={(e) => setGuestEmail(e.target.value)}
        required
      />
      <label htmlFor={`checkin-${roomId}`}>Check-in:</label>
      <input
        id={`checkin-${roomId}`}
        type="date"
        value={checkIn}
        onChange={(e) => setCheckIn(e.target.value)}
        required
      />

      <label htmlFor={`checkout-${roomId}`}>Check-out:</label>
      <input
        id={`checkout-${roomId}`}
        type="date"
        value={checkOut}
        onChange={(e) => setCheckOut(e.target.value)}
        required
      />
      <button type="submit">Book room</button>
      {message && <p role="status">{message}</p>}
    </form>
  );
}

export default BookingForm;
