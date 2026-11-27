import { useState } from "react";
import type { FormEvent } from "react";
import type { CreateRoomDto } from "../types/room";

interface RoomFormProps {
  onSubmit: (dto: CreateRoomDto) => Promise<void>;
  initialValues?: CreateRoomDto;
}

export default function RoomForm({ onSubmit, initialValues }: RoomFormProps) {
  const [name, setName] = useState(initialValues?.name ?? "");
  const [price, setPrice] = useState(initialValues?.price.toString() ?? "");
  const [capacity, setCapacity] = useState(
    initialValues?.capacity.toString() ?? "",
  );

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    await onSubmit({
      name: name.trim(),
      price: Number(price),
      capacity: Number(capacity),
    });
  }

  return (
    <form onSubmit={handleSubmit}>
      <input
        type="text"
        placeholder="Room name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        maxLength={100}
        required
      />

      <input
        type="number"
        placeholder="Price"
        value={price}
        onChange={(e) => setPrice(e.target.value)}
        min="0.01"
        max="100000"
        step="0.01"
        required
      />

      <input
        type="number"
        placeholder="Capacity"
        value={capacity}
        onChange={(e) => setCapacity(e.target.value)}
        min="1"
        max="100"
        step="1"
        required
      />

      <button type="submit">Save Room</button>
    </form>
  );
}
