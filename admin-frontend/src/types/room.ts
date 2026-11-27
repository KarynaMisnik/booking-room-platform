export interface Room {
  id: number;
  name: string;
  price: number;
  capacity: number;
}

export interface CreateRoomDto {
  name: string;
  price: number;
  capacity: number;
}