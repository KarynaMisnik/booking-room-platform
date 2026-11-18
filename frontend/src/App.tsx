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
    <main className="app-main" aria-busy={isLoading}>
      <header className="page-header">
        <p className="page-header__eyebrow">Find your stay</p>
        <h1>Hotel Rooms</h1>
        <p>Choose a comfortable room for your next stay.</p>
      </header>
      {isLoading && <p className="loading-state" role="status"><span className="spinner" aria-hidden="true" />Loading rooms...</p>}
      {error && <p role="alert">{error}</p>}
      {!isLoading && !error && rooms.length === 0 && <p>No rooms available.</p>}
      <section className="rooms-grid" aria-label="Available hotel rooms">
        {rooms.map((room) => <RoomCard key={room.id} room={room} />)}
      </section>
    </main>
  );
}

export default App;
