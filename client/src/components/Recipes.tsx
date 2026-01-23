import axios from "axios";
import { useState } from "react";
import { Button, Card, Container, Form } from "react-bootstrap";
import { useNavigate } from "react-router-dom";

export default function Recipes() {
  const [calories, setCalories] = useState("");
  const [recipes, setRecipes] = useState<Recipes[]>([]);
  const navigate = useNavigate();
  const api = import.meta.env.VITE_API_URL;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    axios
      .get(`${api}/recipe/search?calories=${calories}&number=5`)
      .then((res) => setRecipes(res.data));
  };

  return (
    <Container className="py-4">
      <h1 className="page-title text-center mb-4">Finn oppskrifter</h1>

      <div className="search-form mx-auto" style={{ maxWidth: "500px" }}>
        <Form onSubmit={handleSubmit}>
          <Form.Group className="mb-3">
            <Form.Label>Minimum kalorier</Form.Label>
            <Form.Control
              type="number"
              placeholder="F.eks. 200"
              value={calories}
              onChange={(e) => setCalories(e.target.value)}
            />
          </Form.Group>
          <Button variant="primary" type="submit" className="w-100">
            Søk etter oppskrifter
          </Button>
        </Form>
      </div>

      <div className="row g-3 mt-2">
        {recipes.map((recipe) => (
          <div className="col-6 col-md-4 col-lg-3 col-xl-2" key={recipe.id}>
            <Card className="recipe-card recipe-card-sm h-100">
              <div style={{ overflow: "hidden" }}>
                <Card.Img variant="top" src={recipe.image} />
              </div>
              <Card.Body className="d-flex flex-column p-2">
                <Card.Title className="flex-grow-1">{recipe.title}</Card.Title>
                <Button
                  variant="primary"
                  size="sm"
                  className="mt-2"
                  onClick={() => navigate(`/recipe/${recipe.id}`)}
                >
                  Se oppskrift
                </Button>
              </Card.Body>
            </Card>
          </div>
        ))}
      </div>
    </Container>
  );
}
