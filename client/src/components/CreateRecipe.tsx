import { useState } from "react";
import { Alert, Button, Card, Col, Container, Form, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import agent from "../lib/api/agent";

type Ingredient = {
  name: string;
  amount: string;
};

export default function CreateRecipe() {
  const [title, setTitle] = useState("");
  const [ingredients, setIngredients] = useState<Ingredient[]>([
    { name: "", amount: "" },
  ]);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigate = useNavigate();
  const { isLoggedIn, isLoading } = useAuth();

  const handleAddIngredient = () => {
    setIngredients([...ingredients, { name: "", amount: "" }]);
  };

  const handleRemoveIngredient = (index: number) => {
    if (ingredients.length > 1) {
      setIngredients(ingredients.filter((_, i) => i !== index));
    }
  };

  const handleIngredientChange = (
    index: number,
    field: keyof Ingredient,
    value: string
  ) => {
    const updated = [...ingredients];
    updated[index][field] = value;
    setIngredients(updated);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (!title.trim()) {
      setError("Please enter a title");
      return;
    }

    const validIngredients = ingredients.filter(
      (i) => i.name.trim() && i.amount.trim()
    );

    if (validIngredients.length === 0) {
      setError("Add at least one ingredient with name and amount");
      return;
    }

    setIsSubmitting(true);

    try {
      await agent.post("/createrecipe", {
        title: title.trim(),
        ingredients: validIngredients,
      });
      navigate("/user");
    } catch (err: any) {
      setError(err.response?.data || "Could not create recipe");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <p>Loading...</p>
      </Container>
    );
  }

  if (!isLoggedIn) {
    return (
      <Container className="py-5">
        <Card className="auth-card mx-auto" style={{ maxWidth: "500px" }}>
          <Card.Title>Log in to create a recipe</Card.Title>
          <p className="text-center text-muted mb-4">
            You need to be logged in to create your own recipes
          </p>
          <div className="d-flex gap-2 justify-content-center">
            <Button variant="primary" onClick={() => navigate("/login")}>
              Log In
            </Button>
            <Button variant="outline-secondary" onClick={() => navigate("/signup")}>
              Sign Up
            </Button>
          </div>
        </Card>
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <h1 className="page-title text-center mb-4">Create Your Own Recipe</h1>

      <Card className="create-recipe-card mx-auto" style={{ maxWidth: "700px" }}>
        <Card.Body className="p-4">
          {error && <Alert variant="danger">{error}</Alert>}

          <Form onSubmit={handleSubmit}>
            <Form.Group className="mb-4">
              <Form.Label>Recipe Title</Form.Label>
              <Form.Control
                type="text"
                placeholder="E.g. Grandma's Meatballs"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
              />
            </Form.Group>

            <div className="mb-4">
              <Form.Label>Ingredients</Form.Label>
              {ingredients.map((ingredient, index) => (
                <Row key={index} className="g-2 mb-2 align-items-center">
                  <Col xs={5}>
                    <Form.Control
                      type="text"
                      placeholder="Ingredient"
                      value={ingredient.name}
                      onChange={(e) =>
                        handleIngredientChange(index, "name", e.target.value)
                      }
                    />
                  </Col>
                  <Col xs={4}>
                    <Form.Control
                      type="text"
                      placeholder="Amount"
                      value={ingredient.amount}
                      onChange={(e) =>
                        handleIngredientChange(index, "amount", e.target.value)
                      }
                    />
                  </Col>
                  <Col xs={3}>
                    <Button
                      variant="outline-danger"
                      size="sm"
                      className="w-100"
                      onClick={() => handleRemoveIngredient(index)}
                      disabled={ingredients.length === 1}
                    >
                      Remove
                    </Button>
                  </Col>
                </Row>
              ))}
              <Button
                variant="outline-secondary"
                size="sm"
                className="mt-2"
                onClick={handleAddIngredient}
              >
                + Add Ingredient
              </Button>
            </div>

            <div className="d-grid gap-2">
              <Button variant="primary" type="submit" disabled={isSubmitting}>
                {isSubmitting ? "Creating..." : "Create Recipe"}
              </Button>
              <Button
                variant="outline-secondary"
                onClick={() => navigate(-1)}
                disabled={isSubmitting}
              >
                Cancel
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>
    </Container>
  );
}
