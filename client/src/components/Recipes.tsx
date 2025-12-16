import axios from "axios";
import { useState } from "react";
import { Button, Card, Container, Form, Stack } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function Recipes() {
  const [calories, setCalories] = useState("");
  const [recipes, setRecipes] = useState<Recipes[]>([]);
  const navigate = useNavigate();
  const api = import.meta.env.VITE_API_URL;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    axios
      .get(`${api}/recipe/search?numberOfCalories=${calories}&number=5`)
      .then((res) => setRecipes(res.data));
  };

  return (
    <Container>
      <Form>
        <Form.Group style={{ maxWidth: "300px" }} className="mb-3">
          <Form.Label>Enter min calories </Form.Label>
          <Form.Control
            type="text"
            value={calories}
            onChange={(e) => setCalories(e.target.value)}
          />
        </Form.Group>

        <Button variant="primary" type="submit" onClick={handleSubmit}>
          Submit
        </Button>
      </Form>

      <Stack direction="horizontal" gap={3} className="flex-wrap">
        {recipes.map((recipe) => (
          <Card style={{ width: "18rem", height: "400px" }}>
            <Card.Img variant="top" src={recipe.image} />
            <Card.Body>
              <Card.Title>{recipe.title}</Card.Title>

              <Button
                variant="primary"
                onClick={() => navigate(`/recipe/${recipe.id}`)}
              >
                Read more
              </Button>
            </Card.Body>
          </Card>
        ))}
      </Stack>
    </Container>
  );
}
