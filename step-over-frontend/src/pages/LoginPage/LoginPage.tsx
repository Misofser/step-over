import { useState, useContext } from "react"
import { Navigate } from "react-router"
import { AuthContext, login as loginApi } from "@/features/auth";
import "./LoginPage.css";

export function LoginPage() {
  const { isAuthenticated, loading, login } = useContext(AuthContext);
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  if (loading) {
    return null;
  }

  if (isAuthenticated) {
    return <Navigate to="/today" replace />;
  }

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      const data = await loginApi(username, password);
      login(data);
    } catch {
      setError("Invalid username or password");
    }
  }

  return (
    <div className="login-page">
      <form className="login-form" onSubmit={handleLogin}>
        {error && <div className="error">{error}</div>}
        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={e => setUsername(e.target.value)}
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
      </form>
    </div>
  );
}
