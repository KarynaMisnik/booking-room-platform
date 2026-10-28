import { useEffect, useState } from 'react'
import './App.css'

type Room = {
    id: number
    name: string
    price: number
    capacity: number
}

function App() {
    const [rooms, setRooms] = useState<Room[]>([])
    const [error, setError] = useState('')

    useEffect(() => {
        fetch('http://localhost:5000/api/rooms')
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Failed to fetch rooms')
                }
                return response.json()
            })
            .then((data: Room[]) => setRooms(data))
            .catch(() => setError('Could not load rooms from the backend'))
    }, [])

    return (
        <main>
            <h1>Hotel Rooms</h1>

            {error && <p>{error}</p>}

            {rooms.map((room) => (
                <article key={room.id}>
                    <h2>{room.name}</h2>
                    <p>Price per night: €{room.price}</p>
                    <p>Capacity: {room.capacity} guests</p>
                </article>
            ))}
        </main>
    )
}

export default App