import { Link } from "react-router";
import "./NotFoundPage.css";

export function NotFoundPage() {
  return (
    <div className="notfound-container">
      <h1 className="notfound-title">404 — Page Not Found</h1>
      <p className="notfound-text">
        Sorry, the page you’re looking for doesn’t exist.
      </p>
      <Link to="/" className="notfound-link">
        Go to Home
      </Link>
    </div>
  );
}
