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
<Container 
  className="d-flex flex-column justify-content-center align-items-center my-3" 

>
  <Form>
    <Form.Group className="d-flex flex-column gap-2" style={{ width: "300px" }}>
      <Form.Label>Enter min calories</Form.Label>
      <Form.Control
        type="text"
        value={calories}
        onChange={(e) => setCalories(e.target.value)}
      />
      <Button variant="primary" type="submit" onClick={handleSubmit}>
        Submit
      </Button>
    </Form.Group>
  </Form>

  <Stack
    direction="horizontal"
    gap={3}
    className="flex-wrap justify-content-center mt-4"
  >
    {recipes.map((recipe) => (
      <Card
        style={{ height: "350px" }}
        className="w-25 m-2 d-flex flex-column"
        key={recipe.id}
      >
        <Card.Img variant="top" src={recipe.image} />
        <Card.Body className="d-flex flex-column justify-content-between">
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
