import { useState } from "react";
import { Alert, Button, Card, Form } from "react-bootstrap";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import agent from "../lib/api/agent";
import { useAuth } from "../context/AuthContext";

export default function Login() {
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const { checkAuth } = useAuth();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);
    const loginCredentials: Login = {
      email: email,
      password: password,
    };
    try {
      await agent.post("/auth/login", loginCredentials);
      await checkAuth();
      navigate("/user");
    } catch (err) {
      if (axios.isAxiosError(err) && err.response?.data?.error) {
        setError(err.response.data.error);
      } else {
        setError("An unexpected error occurred");
      }
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center" style={{ minHeight: "80vh" }}>
      <Card className="auth-card mx-3" style={{ maxWidth: "420px", width: "100%" }}>
        <Card.Body>
          <Card.Title>Welcome Back</Card.Title>
          <p className="text-center text-muted mb-4">Log in to continue</p>

          {error && <Alert variant="danger">{error}</Alert>}

          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="formEmail">
              <Form.Label>Email Address</Form.Label>
              <Form.Control
                type="email"
                placeholder="you@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </Form.Group>

            <Form.Group className="mb-4" controlId="formPassword">
              <Form.Label>Password</Form.Label>
              <Form.Control
                type="password"
                placeholder="Enter your password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Form.Group>

            <Button variant="primary" type="submit" className="w-100 mb-3">
              Log In
            </Button>

            <p className="text-center mb-0">
              Don't have an account?{" "}
              <a href="/signUp" className="auth-link">Sign Up</a>
            </p>
          </Form>
        </Card.Body>
      </Card>
    </div>
  );
}
