import axios from "axios";
import { useState } from "react";
import { Button, Card, Form } from "react-bootstrap";
import type { FormEvent } from "react";

export default function Login() {
  const api = import.meta.env.VITE_API_URL;
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const loginCredentials: Login = {
      email: email,
      password: password,
    };
    try {
      const response = await axios.post(`${api}/auth/login`, loginCredentials);
      console.log(response.data);
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
