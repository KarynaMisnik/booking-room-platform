import BookingForm from "./BookingForm";
import type { Room } from "../services/api";
import "./RoomCard.css";

type RoomCardProps = { room: Room };

const imageBaseUrl = (import.meta.env.VITE_IMAGE_BASE_URL ?? "").replace(/\/$/, "");

const roomImages: Record<string, string> = {
  "standard room": "standard-room.jpg",
  "deluxe room": "deluxe-room.jpg",
  "family room": "family-room.jpg",
  "superior room": "superior-room.jpg",
  "luxury suite": "luxury-suite.jpg",
};

function RoomCard({ room }: RoomCardProps) {
  const imageFile = roomImages[room.name.toLowerCase()] ?? "standard-room.jpg";
  const image = `${imageBaseUrl}/images/${imageFile}`;

  return (
    <article className="room-card">
      <img className="room-card__image" src={image} alt={`${room.name} interior`} />
      <div className="room-card__content">
        <div>
          <p className="room-card__eyebrow">Hotel accommodation</p>
          <h2>{room.name}</h2>
          <div className="room-card__details">
            <p className="room-card__price"><strong>€{room.price}</strong> <span>per night</span></p>
            <p>{room.capacity} guests</p>
          </div>
        </div>
        <BookingForm roomId={room.id} />
      </div>
    </article>
  );
}

export default RoomCard;
