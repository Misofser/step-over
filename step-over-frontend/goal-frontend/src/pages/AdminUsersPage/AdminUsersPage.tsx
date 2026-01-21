import { useEffect, useState, useContext } from "react"
import "./AdminUsersPage.css"
import { fetchUsers, addUser, deleteUser } from "../../api/users"
import { Modal } from "../../components/Modal/Modal"
import { Button } from "../../components/Button/Button"
import { UserForm } from "../../components/UserForm/UserForm"
import { AuthContext } from "../../auth/AuthContext"
import type { User } from "../../api/users.types"
import { EditUserForm } from "../../components/EditUserForm/EditUserForm"

export function AdminUsersPage() {
  const [users, setUsers] = useState<User[]>([]);
  const [showModal, setShowModal] = useState(false);
  const { user: currentUser } = useContext(AuthContext);

  const [editingUserId, setEditingUserId] = useState<number | null>(null);

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

  const handleSavedUser = (id: number, newUsername: string) => {
    setUsers(prev =>
      prev.map(g =>
        g.id === id ? { ...g, username: newUsername } : g
      )
    );
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
                {user.id !== currentUser?.id && (
                  <div className="actions-block">
                    <Button
                      variant="edit"
                      onClick={() => setEditingUserId(user.id)}
                    >
                      ✏️
                    </Button>
                    <Button
                      onClick={() => handleDelete(user.id)}
                      variant="delete"
                    >❌</Button>
                  </div>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {showModal && <Modal title="Add New User" >
        <UserForm
          onSubmit={handleCreateUser}
          onCancel={() => setShowModal(false)}/>
      </Modal>}
      {editingUserId && (
        <Modal title="Edit User">
          <EditUserForm
            userId={editingUserId}
            onClose={() => setEditingUserId(null)}
            onSave={handleSavedUser}
          />
        </Modal>
      )}
    </div>
  );
}
