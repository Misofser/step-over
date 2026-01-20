import { useEffect, useState } from "react"
import "./AdminUsersPage.css"
import { fetchUsers, addUser, deleteUser } from "../../api/users"
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

  const handleDelete = async (id: number) => {
    await deleteUser(id);
    setUsers(users.filter((u) => u.id !== id));
  };

  return (
    <div className="app-container">
      <h1>Users</h1>
      <div className="users-btn-block">
        <Button onClick={() => setShowModal(true)}>Add User</Button>
      </div>
      <table className="users-table">
        <colgroup>
          <col className="id-col" />
          <col className="username-col" />
          <col className="role-col" />
          <col className="actions-col" />
        </colgroup>
        <thead>
          <tr>
            <th className="id-col">ID</th>
            <th>Username</th>
            <th>Role</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {users.map((user) => (
            <tr key={user.id}>
              <td className="id-col" data-label="Id">{user.id}</td>
              <td data-label="Username">{user.username}</td>
              <td data-label="Role">{user.role}</td>
              <td data-label="Actions">
                <Button
                  onClick={() => handleDelete(user.id)}
                  variant="delete"
                >❌</Button>
              </td>
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
