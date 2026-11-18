import { useState, type FormEvent } from "react";
import { ApiError, createBooking } from "../services/api";

type BookingFormProps = {
  roomId: number;
  roomName: string;
};

type Confirmation = {
  id: number;
  roomName: string;
  guestName: string;
  checkIn: string;
  checkOut: string;
};

function formatDate(date: string) {
  return new Intl.DateTimeFormat(undefined, { dateStyle: "medium" }).format(
    new Date(`${date.slice(0, 10)}T00:00:00`),
  );
}

function BookingForm({ roomId, roomName }: BookingFormProps) {
  const [guestName, setGuestName] = useState("");
  const [guestEmail, setGuestEmail] = useState("");
  const [checkIn, setCheckIn] = useState("");
  const [checkOut, setCheckOut] = useState("");
  const [message, setMessage] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [confirmation, setConfirmation] = useState<Confirmation | null>(null);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage("");
    setErrorMessage("");
    setConfirmation(null);

    const booking = {
      roomId,
      guestName,
      guestEmail,
      checkIn: `${checkIn}T15:00:00Z`,
      checkOut: `${checkOut}T11:00:00Z`,
    };

    try {
      const savedBooking = await createBooking(booking);
      setConfirmation({
        id: savedBooking.id,
        roomName,
        guestName: savedBooking.guestName,
        checkIn: savedBooking.checkIn,
        checkOut: savedBooking.checkOut,
      });
      setGuestName("");
      setGuestEmail("");
      setCheckIn("");
      setCheckOut("");
      setMessage("Booking created successfully!");
    } catch (error) {
      console.error("Booking request failed:", error);
      if (error instanceof ApiError) {
        const messages: Record<number, string> = {
          400: "The selected dates are invalid. Please choose a check-out date after check-in.",
          404: "This room could not be found. Please refresh and choose another room.",
          409: "This room is already booked for those dates. Please choose different dates or another room.",
        };
        setErrorMessage(messages[error.status] ?? error.message);
      } else {
        setErrorMessage("We could not reach the booking service. Please try again later.");
      }
    }
  }

  return (
    <>
      {confirmation && (
        <section className="booking-confirmation" role="status" aria-live="polite">
          <h3>Reservation confirmed</h3>
          <p>{message}</p>
          <dl>
            <div><dt>Booking reference</dt><dd>#{confirmation.id}</dd></div>
            <div><dt>Room</dt><dd>{confirmation.roomName}</dd></div>
            <div><dt>Guest</dt><dd>{confirmation.guestName}</dd></div>
            <div><dt>Dates</dt><dd>{formatDate(confirmation.checkIn)} – {formatDate(confirmation.checkOut)}</dd></div>
          </dl>
        </section>
      )}

      <form onSubmit={handleSubmit}>
      <h3>Book {roomName}</h3>

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
      {!confirmation && errorMessage && <p className="booking-error" role="alert">{errorMessage}</p>}
      </form>
    </>
  );
}

export default BookingForm;
