import { useEffect, useRef, useState } from "react";
import { Modal } from "../../../../components/Modal/Modal";
import { Button } from "../../../../components/Button/Button";
import { ChangePasswordForm } from "../../../auth";
import "./UserDropdown.css";

type UserDropdownProps = {
  username: string;
  logout: () => void;
};

export function UserDropdown({
  username,
  logout,
}: UserDropdownProps) {
  const [open, setOpen] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const dropdownRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    const handlePointerDown = (event: PointerEvent) => {
      if (!dropdownRef.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    };

    document.addEventListener("pointerdown", handlePointerDown);

    return () => {
      document.removeEventListener("pointerdown", handlePointerDown);
    };
  }, [open]);

  return (
    <div ref={dropdownRef} className="user-dropdown">
      <button
        type="button"
        className="navbar-button"
        onClick={() => setOpen((prev) => !prev)}
      >
        Hi, {username}! ▼
      </button>

      {open && (
        <div className="user-dropdown-menu">
          <Button
            variant="edit"
            onClick={() => {
              setOpen(false);
              setShowPasswordModal(true);
            }}
          >
            Change password
          </Button>

          <Button
            variant="delete"
            onClick={() => {
              setOpen(false);
              logout();
            }}
          >
            Logout
          </Button>
        </div>
      )}

      {showPasswordModal && (
        <Modal
          title="Change password"
          onClose={() => setShowPasswordModal(false)}
        >
          <ChangePasswordForm
            onSuccess={() => setShowPasswordModal(false)}
            onCancel={() => setShowPasswordModal(false)}
          />
        </Modal>
      )}
    </div>
  );
}
