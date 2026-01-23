import { useState } from "react";
import { Button, Card, Form } from "react-bootstrap";
import type { FormEvent } from "react";
import { useNavigate } from "react-router-dom";
import agent from "../lib/api/agent";
import { useAuth } from "../context/AuthContext";

export default function SignUp() {
  const navigate = useNavigate();
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [userName, setUserName] = useState("");
  const { checkAuth } = useAuth();

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const data: SignUp = {
      email: email,
      password: password,
      userName: userName
    };
    try {
      await agent.post("/auth/register", data);
      await checkAuth();
      navigate("/userPage");
    } catch (error) {
      console.log(error);
    }
  };

  return (
    <Card className="p-4 mt-5 shadow-sm mx-auto" style={{ maxWidth: "400px" }}>
      <Card.Body>
        <Card.Title>Login</Card.Title>
        <Form className="d-flex flex-column gap-3" onClick={handleSubmit}>
          <Form.Group controlId="formEmail">
            <Form.Label>Email address</Form.Label>
            <Form.Control
              type="email"
              placeholder="Enter email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </Form.Group>

        <Form.Group controlId="userName">
            <Form.Label>User Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter your username"
              value={userName}
              onChange={(e) => setUserName(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="formPassword">
            <Form.Label>Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="Password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </Form.Group>

          <Button variant="primary" type="submit">
            Submit
          </Button>
        </Form>
      </Card.Body>
    </Card>
  );
}


