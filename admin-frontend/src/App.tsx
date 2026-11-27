import type { User } from "./types/auth";
import LoginPage from "./pages/LoginPage";
import { useState, useEffect } from "react";
import { logout, getCurrentUser } from "./services/authService";
import RoomManagementPage from "./pages/RoomManagementPage";

function App() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);

  async function handleLogout() {
    try {
      await logout();
      setUser(null);
    } catch {
      alert("Logout failed");
    }
  }

  useEffect(() => {
    async function restoreSession() {
      try {
        const currentUser = await getCurrentUser();
        setUser(currentUser);
      } catch (error) {
        console.error("Failed to restore session", error);
      } finally {
        setLoading(false);
      }
    }

    restoreSession();
  }, []);

  if (loading) {
    return <p>Loading...</p>;
  }

  if (!user) {
    return <LoginPage onLogin={setUser} />;
  }

  return (
    <div>
      <h1>Hotel Staff Dashboard</h1>
      <p>Welcome, {user.username}</p>
      <p>Role: {user.role}</p>

      {user.role === "Admin" && <RoomManagementPage />}

      {user.role === "Manager" && <h2>Booking Management</h2>}
      <button onClick={handleLogout}>Logout</button>
    </div>
  );
}

export default App;
