import axios from "axios";
import { useEffect, useState } from "react";
import { Col, Container, Row, Spinner } from "react-bootstrap";
import { useParams } from "react-router-dom";
import type { Recipe } from "../lib/api";
import Comments from "./Comments";
import { UtensilsCrossed } from "lucide-react";

export default function RecipeDetail() {
  const [recipe, setRecipe] = useState<Recipe>();
  const [isLoading, setIsLoading] = useState(true);
  const { id } = useParams();
  const api = import.meta.env.VITE_API_URL;

  useEffect(() => {
    axios
      .get(`${api}/recipe/${id}`)
      .then((res) => setRecipe(res.data))
      .catch(console.error)
      .finally(() => setIsLoading(false));
  }, [id, api]);

  if (isLoading) {
    return (
      <Container className="py-5 text-center">
        <Spinner style={{ color: "var(--primary-color)", width: "2.5rem", height: "2.5rem" }} />
      </Container>
    );
  }

  return (
    <Container className="py-4">
      <Row>
        <Col md={10} lg={8} xl={6} className="mx-auto">
          <div style={{
            background: "var(--card-bg)",
            borderRadius: "var(--border-radius)",
            overflow: "hidden",
            border: "1px solid var(--border-color)",
            boxShadow: "var(--shadow-md)"
          }}>
            {recipe?.image ? (
              <img
                src={recipe.image}
                alt={recipe.title}
                style={{ width: "100%", height: "260px", objectFit: "cover" }}
              />
            ) : (
              <div style={{
                width: "100%", height: "200px", background: "linear-gradient(135deg, #fdeee4, #fad9c2)",
                display: "flex", alignItems: "center", justifyContent: "center", color: "var(--primary-color)"
              }}>
                <UtensilsCrossed size={56} strokeWidth={1.25} />
              </div>
            )}

            <div style={{ padding: "1.5rem" }}>
              <h1 style={{ fontSize: "1.6rem", color: "var(--text-primary)", fontWeight: 700, fontFamily: "'Playfair Display', serif" }} className="mb-3">
                {recipe?.title}
              </h1>

              <div
                className="text-muted mb-3 pb-3 border-bottom"
                style={{ fontSize: "0.92rem", lineHeight: 1.7 }}
                dangerouslySetInnerHTML={{ __html: recipe?.summary || "" }}
              />

              <div className="mb-4">
                <h2 style={{ color: "var(--primary-color)", fontWeight: 600, fontSize: "1.05rem" }} className="mb-2">
                  Ingredients
                </h2>
                <ul style={{ paddingLeft: "1.25rem", fontSize: "0.92rem" }}>
                  {recipe?.extendedIngredients.map((ing, index) => (
                    <li key={index} className="mb-1">{ing}</li>
                  ))}
                </ul>
              </div>

              <div>
                <h2 style={{ color: "var(--primary-color)", fontWeight: 600, fontSize: "1.05rem" }} className="mb-2">
                  Instructions
                </h2>
                <div
                  style={{ fontSize: "0.92rem", lineHeight: 1.7 }}
                  dangerouslySetInnerHTML={{ __html: recipe?.instructions || "" }}
                />
              </div>
            </div>
          </div>

          {id && <Comments recipeId={parseInt(id)} />}
        </Col>
      </Row>
    </Container>
  );
}
