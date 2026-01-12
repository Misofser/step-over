import { useContext } from "react";
import { Link } from "react-router";

import { AuthContext } from "../../auth/AuthContext";
import "./Navbar.css";

export const Navbar = () => {
  const { userRole, isAuthenticated, logout } = useContext(AuthContext);

  return (
    <nav className="navbar">
      <div className="navbar-left">
        <Link to="/" className="navbar-logo">StepOver</Link>
      </div>
      <div className="navbar-right">
        {isAuthenticated ? (
          <>
            <span className="navbar-user">Hi, {userRole}!</span>
            <button onClick={logout} className="navbar-button">Logout</button>
          </>
        ) : (
          <Link to="/login" className="navbar-button">Login</Link>
        )}
      </div>
    </nav>
  );
};
