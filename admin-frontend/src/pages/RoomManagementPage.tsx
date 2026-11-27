import { useState, useEffect } from "react";
import type { Room, CreateRoomDto } from "../types/room";
import {
  getRooms,
  createRoom,
  deleteRoom,
  updateRoom,
} from "../services/roomService";
import RoomForm from "../components/RoomForm";

export default function RoomManagementPage() {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [error, setError] = useState("");
  const [selectedRoom, setSelectedRoom] = useState<Room | null>(null);

  useEffect(() => {
    async function loadRooms() {
      try {
        const data = await getRooms();
        setRooms(data);
      } catch {
        setError("Failed to load rooms");
      }
    }

    loadRooms();
  }, []);

  async function handleCreateRoom(dto: CreateRoomDto) {
    setError("");

    try {
      const newRoom = await createRoom(dto);

      setRooms((previousRooms) => [...previousRooms, newRoom]);
    } catch {
      setError("Failed to create room");
    }
  }

  async function handleUpdateRoom(dto: CreateRoomDto) {
    if (!selectedRoom) return;

    setError("");

    try {
      await updateRoom(selectedRoom.id, dto);

      setRooms((previousRooms) =>
        previousRooms.map((room) =>
          room.id === selectedRoom.id ? { ...room, ...dto } : room,
        ),
      );

      setSelectedRoom(null);
    } catch {
      setError("Failed to update room");
    }
  }

  async function handleDeleteRoom(id: number) {
    if (!window.confirm("Are you sure you want to delete this room?")) {
      return;
    }

    setError("");

    try {
      await deleteRoom(id);

      setRooms((previousRooms) =>
        previousRooms.filter((room) => room.id !== id),
      );
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Failed to delete room",
      );
    }
  }

  return (
    <div>
      <h2>Room Management</h2>

      {error && <p>{error}</p>}

      <h3>{selectedRoom ? "Edit Room" : "Add New Room"}</h3>

      <RoomForm
        key={selectedRoom?.id ?? "create"}
        initialValues={selectedRoom ?? undefined}
        onSubmit={selectedRoom ? handleUpdateRoom : handleCreateRoom}
      />

      {selectedRoom && (
        <button onClick={() => setSelectedRoom(null)}>Cancel</button>
      )}

      <table>
        <thead>
          <tr>
            <th>ID</th>
            <th>Name</th>
            <th>Price</th>
            <th>Capacity</th>
            <th>Actions</th>
          </tr>
        </thead>

        <tbody>
          {rooms.map((room) => (
            <tr key={room.id}>
              <td>{room.id}</td>
              <td>{room.name}</td>
              <td>{room.price} €</td>
              <td>{room.capacity}</td>
              <td>
                <button onClick={() => setSelectedRoom(room)}>Edit</button>
                <button onClick={() => handleDeleteRoom(room.id)}>
                  Delete
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
