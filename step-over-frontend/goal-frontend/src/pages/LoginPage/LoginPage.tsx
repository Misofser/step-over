import { useState, useContext } from "react"
import { Navigate, useNavigate } from "react-router"
import { AuthContext } from "../../auth/AuthContext";
import { login as loginApi } from "../../api/auth";
import "./LoginPage.css";

export function LoginPage() {
  const { isAuthenticated, loading, login } = useContext(AuthContext);
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  if (loading) {
    return null;
  }

  if (isAuthenticated) {
    return <Navigate to="/goals" replace />;
  }

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setError("");

    try {
      const data = await loginApi(username, password);
      login(data);
      navigate("/goals");
    } catch (err: any) {
      setError(err.message || "Invalid username or password");
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
