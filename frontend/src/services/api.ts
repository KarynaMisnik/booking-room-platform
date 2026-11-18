export type Room = {
  id: number;
  name: string;
  price: number;
  capacity: number;
};

type RoomApiResponse = {
  id: number;
  name: string;
  price: number;
  capacity: number;
};

export type CreateBookingRequest = {
  roomId: number;
  guestName: string;
  guestEmail: string;
  checkIn: string;
  checkOut: string;
};

const apiBaseUrl = (
  import.meta.env.VITE_API_BASE_URL ?? "/api"
).replace(/\/$/, "");

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    headers: {
      "Content-Type": "application/json",
      ...options?.headers,
    },
    ...options,
  });

  if (!response.ok) {
    throw new Error(`API request failed with status ${response.status}`);
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function mapRoom(response: RoomApiResponse): Room {
  return {
    id: response.id,
    name: response.name,
    price: Number(response.price),
    capacity: response.capacity,
  };
}

export async function getRooms(): Promise<Room[]> {
  const rooms = await request<RoomApiResponse[]>("/rooms");
  return rooms.map(mapRoom);
}

export async function createBooking(
  booking: CreateBookingRequest,
): Promise<void> {
  await request<unknown>("/bookings", {
    method: "POST",
    body: JSON.stringify(booking),
  });
}
