import { useEffect, useState } from "react"
import "./AdminUsersPage.css"
import { fetchUsers, addUser } from "../../api/users"
import { Modal } from "../../components/Modal/Modal"
import { Button } from "../../components/Button/Button"
import { UserForm } from "../../components/UserForm/UserForm"

interface User {
  id: number;
  username: string;
  role: string;
}

export function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    async function load() {
      const data = await fetchUsers();
      setUsers(data);
    }
    load();
  }, []);

  const handleCreateUser = async (data: { username: string; password: string;}) => {
    const newUser = await addUser(data);
    setUsers((prev) => [...prev, newUser]);
    setShowModal(false);
  };

  return (
    <div className="app-container">
      <h1>Users</h1>
      <div className="users-btn-block">
        <Button onClick={() => setShowModal(true)}>Add User</Button>
      </div>
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
      {showModal && <Modal title="Add New User" >
        <UserForm onSubmit={handleCreateUser} onCancel={() => setShowModal(false)}/>
      </Modal>}
    </div>
  );
}
