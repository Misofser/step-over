import { useState } from "react"
import "./UserForm.css"

type UserFormProps = {
  onSubmit: (data: { username: string; password: string; 
}) => void;
  onCancel?: () => void;
};

export function UserForm({ onSubmit, onCancel }: UserFormProps) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async () => {
    if (!username.trim() || !password.trim()) {
      setError("Username and password are required");
      return;
    }

    await onSubmit({ username: username.trim(), password: password.trim()});
    setUsername("");
    setPassword("");
    setError("");
  };

  return (
    <div className="user-form">
      <div className="user-form-fields">
        <input
          placeholder="Username"
          value={username}
          onChange={(e) => {setUsername(e.target.value)}}
        />
        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
      </div>

      {error && <p className="error-text">{error}</p>}

      <div className="actions">
        <button onClick={handleSubmit} className="primary">Create</button>
        {onCancel && (
          <button onClick={onCancel} className="secondary">Cancel</button>
        )}
      </div>
    </div>
  );
}
