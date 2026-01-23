import { useState } from "react";
import { Alert, Button, Card, Form } from "react-bootstrap";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import agent from "../lib/api/agent";
import { useAuth } from "../context/AuthContext";

export default function SignUp() {
  const navigate = useNavigate();
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [userName, setUserName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const { checkAuth } = useAuth();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);
    const data: SignUp = {
      email: email,
      password: password,
      userName: userName
    };
    try {
      await agent.post("/auth/register", data);
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
          <Card.Title>Opprett konto</Card.Title>
          <p className="text-center text-muted mb-4">Bli med i NomNom i dag</p>

          {error && <Alert variant="danger">{error}</Alert>}

          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-3" controlId="userName">
              <Form.Label>Brukernavn</Form.Label>
              <Form.Control
                type="text"
                placeholder="Velg et brukernavn"
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
              />
            </Form.Group>

            <Form.Group className="mb-3" controlId="formEmail">
              <Form.Label>E-postadresse</Form.Label>
              <Form.Control
                type="email"
                placeholder="din@epost.no"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
            </Form.Group>

            <Form.Group className="mb-4" controlId="formPassword">
              <Form.Label>Passord</Form.Label>
              <Form.Control
                type="password"
                placeholder="Lag et sterkt passord"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
            </Form.Group>

            <Button variant="primary" type="submit" className="w-100 mb-3">
              Registrer deg
            </Button>

            <p className="text-center mb-0">
              Har du allerede konto?{" "}
              <a href="/login" className="auth-link">Logg inn</a>
            </p>
          </Form>
        </Card.Body>
      </Card>
    </div>
  );
}


