import { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/main.css";
import logo from "../assets/logo.png";
import { login } from "../services/auth";

export default function Login() {
  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const role = await login(username, password);

      if (role.toLowerCase() === "admin") navigate("/admin/dashboard");
      else if (role.toLowerCase() === "employee") navigate("/employee/dashboard");
      else navigate("/");
      
    } catch (err) {
      setError("Login failed: " + err.message);
    }
  };

  return (
    <div className="auth-container">
      <form className="auth-box" onSubmit={handleLogin}>
        <img src={logo} alt="App Logo" className="logo" />
        <h2>Login</h2>

        {error && <p className="error">{error}</p>}

        <input
          type="text"
          placeholder="Username"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
          required
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <button type="submit">Login</button>
      </form>
    </div>
  );
}
