import { useContext } from "react";
import { Link, NavLink } from "react-router";

import { AuthContext } from "@/features/auth";
import { UserDropdown } from "@/features/users";
import "./Navbar.css";

const getNavLinkClass = ({ isActive }: { isActive: boolean }) =>
  isActive ? "nav-link active" : "nav-link";

export const Navbar = () => {
  const { user, isAuthenticated, logout } = useContext(AuthContext);

  return (
    <nav className="navbar">
      <div className="navbar-left">
        <Link to="/" className="navbar-logo">StepOver</Link>
      </div>
      {isAuthenticated && user && (
        <div className="links">
          <NavLink to="/today" className={getNavLinkClass}>
            Today
          </NavLink>
          <NavLink to="/goals" className={getNavLinkClass}>
            Goals
          </NavLink>
          {user.role === "Admin" && (
            <NavLink to="/admin/users" className={getNavLinkClass}>
              Users
            </NavLink>
          )}
        </div>
      )}
      <div className="navbar-right">
        {isAuthenticated && user ? (
          <UserDropdown
            username={user.username}
            logout={logout}
          />
        ) : (
          <Link to="/login" className="navbar-button">Login</Link>
        )}
      </div>
    </nav>
  );
};
