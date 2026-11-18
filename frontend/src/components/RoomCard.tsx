import BookingForm from "./BookingForm";
import type { Room } from "../services/api";
import standardRoomImage from "../assets/rooms/standard.jpg";
import deluxeRoomImage from "../assets/rooms/deluxe.jpg";
import familyRoomImage from "../assets/rooms/family.jpg";
import superiorRoomImage from "../assets/rooms/superior.jpg";
import luxurySuiteImage from "../assets/rooms/luxury-suite.jpg";
import "./RoomCard.css";

type RoomCardProps = { room: Room };

const roomImages: Record<string, string> = {
  "standard room": standardRoomImage,
  "deluxe room": deluxeRoomImage,
  "family room": familyRoomImage,
  "superior room": superiorRoomImage,
  "luxury suite": luxurySuiteImage,
};

function RoomCard({ room }: RoomCardProps) {
  const image = roomImages[room.name.toLowerCase()] ?? standardRoomImage;

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
