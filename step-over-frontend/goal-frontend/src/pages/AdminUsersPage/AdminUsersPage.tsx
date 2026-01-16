import { useEffect, useState } from "react"
import "./AdminUsersPage.css"
import { fetchUsers } from "../../api/users"

interface User {
  id: number;
  username: string;
  role: string;
}

export function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);

  useEffect(() => {
    async function load() {
      const data = await fetchUsers();
      setUsers(data);
    }
    load();
  }, []);

  return (
    <div className="app-container">
      <h1>Users</h1>
      <table className="users-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Role</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.username}</td>
              <td>{user.role}</td>
            </tr>
          ))}
        </tbody> 
      </table>
    </div>
  );
}
