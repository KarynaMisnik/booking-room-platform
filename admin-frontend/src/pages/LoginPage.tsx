import { useState } from "react";
import type { FormEvent } from "react";
import type { User } from "../types/auth";
import { login } from "../services/authService";

interface LoginPageProps {
  onLogin: (user: User) => void;
}

export default function LoginPage({ onLogin }: LoginPageProps) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    try {
      const user = await login({ username, password });
      onLogin(user);
    } catch {
      setError("Login failed. Check your credentials.");
    }
  }

  return (
    <div>
      <h1>Hotel Staff Login</h1>

      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <button type="submit">Login</button>

        {error && <p>{error}</p>}
      </form>
    </div>
  );
}
