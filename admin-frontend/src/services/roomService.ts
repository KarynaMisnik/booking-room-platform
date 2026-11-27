import type { Room, CreateRoomDto } from "../types/room";
import { getCsrfToken } from "./authService";
const API_URL = "/api/admin/rooms";

export async function getRooms(): Promise<Room[]> {
  const response = await fetch(API_URL, {
    credentials: "include",
  });

  if (!response.ok) {
    throw new Error("Failed to retrieve rooms");
  }

  return response.json();
}

export async function createRoom(dto: CreateRoomDto): Promise<Room> {
  const token = await getCsrfToken();

  const response = await fetch(API_URL, {
    method: "POST",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      "X-CSRF-TOKEN": token,
    },
    body: JSON.stringify(dto),
  });

  if (!response.ok) {
    throw new Error("Failed to create room");
  }

  return response.json();
}

export async function deleteRoom(id: number): Promise<void> {
  const token = await getCsrfToken();

  const response = await fetch(`${API_URL}/${id}`, {
    method: "DELETE",
    credentials: "include",
    headers: {
      "X-CSRF-TOKEN": token,
    },
  });

  if (response.status === 409) {
    throw new Error("Cannot delete a room with existing bookings.");
  }

  if (!response.ok) {
    throw new Error("Failed to delete room");
  }
}

export async function updateRoom(
  id: number,
  dto: CreateRoomDto
): Promise<void> {
  const token = await getCsrfToken();

  const response = await fetch(`${API_URL}/${id}`, {
    method: "PUT",
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      "X-CSRF-TOKEN": token,
    },
    body: JSON.stringify(dto),
  });

  if (!response.ok) {
    throw new Error("Failed to update room");
  }
}