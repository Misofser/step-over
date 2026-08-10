import { useState, useEffect } from "react"
import type { User } from "../../api/users.types"
import { fetchUser, updateUser } from "../../api/users"
import "./EditUserForm.css"

interface EditUserFormProps {
  userId: number;
  onClose: () => void;
  onSave: (id: number, newTitle: string) => void;
}

export function EditUserForm({ userId, onClose, onSave }: EditUserFormProps) {
  const [user, setUser] = useState<User | null>(null);
  const [username, setUsername] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadUser() {
      const data = await fetchUser(userId);
      setUser(data);
      setUsername(data.username);
    }
    loadUser();
  }, [userId]);

  const handleSave = async () => {
    if (!username.trim()) {
      setError("Username cannot be empty");
      return;
    }
    await updateUser(userId, { username: username.trim() });
    onSave(userId, username.trim());
    onClose();
  };

  if (!user) {
    return (
      <div className="modal-backdrop">
        <div className="modal">
          <p>Loading...</p>
        </div>
      </div>
    );
  };

  return (
    <div className="edit-user-form">
      <input
        value={username}
        onChange={e => {
          setUsername(e.target.value);
          setError("");
        }}
        onKeyDown={(e) => e.key === "Enter" && handleSave()}
        className={error ? "error" : ""}
      />

      {error && <p className="error-text">{error}</p>}

      <div className="actions">
        <button onClick={handleSave} className="primary">Save</button>
        <button onClick={onClose} className="secondary">Cancel</button>
      </div>
    </div>
  );
}
