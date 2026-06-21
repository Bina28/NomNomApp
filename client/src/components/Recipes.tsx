import axios from "axios";
import { useState } from "react";
import { Button, Card, Container, Form, Spinner } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import type { Recipe } from "../lib/api";
import { UtensilsCrossed, ChevronLeft, ChevronRight } from "lucide-react";

export default function Recipes() {
  const [calories, setCalories] = useState("");
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [hasSearched, setHasSearched] = useState(false);
  const [page, setPage] = useState(1);
  const [hasNextPage, setHasNextPage] = useState(false);
  const navigate = useNavigate();
  const api = import.meta.env.VITE_API_URL;

  const fetchRecipes = async (pageNumber: number) => {
    setIsLoading(true);
    try {
      const res = await axios.get(`${api}/recipe/search?calories=${calories}&number=8&pageNumber=${pageNumber}&pageSize=8`);
      setRecipes(res.data.items ?? []);
      setHasNextPage(res.data.hasNextPage ?? false);
      setPage(pageNumber);
    } finally {
      setIsLoading(false);
      setHasSearched(true);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    fetchRecipes(1);
  };

  return (
    <Container className="py-4">
      <h1 className="page-title text-center mb-4">Find Recipes</h1>

      <div className="search-form mx-auto" style={{ maxWidth: "500px" }}>
        <Form onSubmit={handleSubmit}>
          <Form.Group className="mb-3">
            <Form.Label>Minimum Calories</Form.Label>
            <Form.Control
              type="number"
              placeholder="E.g. 200"
              value={calories}
              onChange={(e) => setCalories(e.target.value)}
              required
              min={1}
            />
          </Form.Group>
          <Button variant="primary" type="submit" className="w-100" disabled={isLoading}>
            {isLoading ? <Spinner size="sm" className="me-2" /> : null}
            Search Recipes
          </Button>
        </Form>
      </div>

      {isLoading && (
        <div className="text-center py-5">
          <Spinner style={{ color: "var(--primary-color)", width: "2.5rem", height: "2.5rem" }} />
        </div>
      )}

      {!isLoading && hasSearched && recipes.length === 0 && (
        <div className="text-center py-5" style={{ color: "var(--text-secondary)" }}>
          <UtensilsCrossed size={48} strokeWidth={1} style={{ marginBottom: "1rem", opacity: 0.4 }} />
          <p className="mb-0">No recipes found. Try a different calorie amount.</p>
        </div>
      )}

      {!isLoading && recipes.length > 0 && (
        <>
          <div className="row g-3 mt-2 justify-content-center">
            {recipes.map((recipe) => (
              <div className="col-6 col-md-4 col-lg-3" key={recipe.id}>
                <Card className="recipe-card recipe-card-sm h-100">
                  {recipe.image ? (
                    <div style={{ overflow: "hidden" }}>
                      <Card.Img variant="top" src={recipe.image} />
                    </div>
                  ) : (
                    <div className="recipe-card-no-img recipe-card-no-img-sm">
                      <UtensilsCrossed size={28} strokeWidth={1.5} />
                    </div>
                  )}
                  <Card.Body className="d-flex flex-column p-2">
                    <Card.Title className="flex-grow-1">{recipe.title}</Card.Title>
                    <Button
                      variant="primary"
                      size="sm"
                      className="mt-2"
                      onClick={() => navigate(`/recipe/${recipe.id}`)}
                    >
                      View Recipe
                    </Button>
                  </Card.Body>
                </Card>
              </div>
            ))}
          </div>

          <div className="d-flex justify-content-center align-items-center gap-3 mt-4">
            <Button
              variant="outline-secondary"
              size="sm"
              disabled={page === 1}
              onClick={() => fetchRecipes(page - 1)}
            >
              <ChevronLeft size={16} /> Previous
            </Button>
            <span style={{ color: "var(--text-secondary)", fontSize: "0.9rem" }}>Page {page}</span>
            <Button
              variant="outline-secondary"
              size="sm"
              disabled={!hasNextPage}
              onClick={() => fetchRecipes(page + 1)}
            >
              Next <ChevronRight size={16} />
            </Button>
          </div>
        </>
      )}
    </Container>
  );
}
