import { useEffect, useState } from "react";
import "./App.css";
import RoomCard from "./components/RoomCard";
import { getRooms, type Room } from "./services/api";

function App() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    getRooms()
      .then(setRooms)
      .catch(() => setError("Could not load rooms from the backend"))
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <main>
      <h1>Hotel Rooms</h1>
      {isLoading && <p role="status">Loading rooms...</p>}
      {error && <p role="alert">{error}</p>}
      {!isLoading && !error && rooms.length === 0 && <p>No rooms available.</p>}
      <section className="rooms-grid" aria-label="Available hotel rooms">
        {rooms.map((room) => <RoomCard key={room.id} room={room} />)}
      </section>
    </main>
  );
}

export default App;
