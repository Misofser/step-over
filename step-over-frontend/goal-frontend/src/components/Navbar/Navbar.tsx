import { useContext } from "react";
import { Link, NavLink } from "react-router";

import { AuthContext } from "../../features/auth";
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
          <>
            <span className="navbar-user">Hi, {user.username}!</span>
            <button onClick={logout} className="navbar-button">Logout</button>
          </>
        ) : (
          <Link to="/login" className="navbar-button">Login</Link>
        )}
      </div>
    </nav>
  );
};
