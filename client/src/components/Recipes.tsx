import axios from "axios";
import { useState } from "react";
import { Button, Card, Container, Form } from "react-bootstrap";

export default function Recipes() {
  const [calories, setCalories] = useState("");
  const [recipes, setRecipes] = useState<Recipes[]>([]);

  const apiKey = import.meta.env.VITE_SPOONACULAR_KEY;

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    axios
      .get(
        `https://api.spoonacular.com/recipes/findByNutrients?apiKey=${apiKey}&minCalories=${calories}&number=5`
      )
      .then((res) => setRecipes(res.data));
  };

  return (
    <Container>
      <div className="d-flex">
        <Form>
          <Form.Group className="mb-3">
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
          {recipes.map((recipe) => (
            <Card style={{ width: "18rem" }}>
              <Card.Img variant="top" src="holder.js/100px180" />
              <Card.Body>
                <Card.Title>{recipe.id}</Card.Title>
                <Card.Text>
                  Some quick example text to build on the card title and make up
                  the bulk of the card's content.
                </Card.Text>
                <Button variant="primary">Go somewhere</Button>
              </Card.Body>
            </Card>
          ))}
        </Form>
      </div>
    </Container>
  );
}
