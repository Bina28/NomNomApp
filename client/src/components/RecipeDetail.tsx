import axios from "axios";
import { useEffect, useState } from "react";
import { Col, Container, Row } from "react-bootstrap";
import { useParams } from "react-router-dom";
import Comments from "./Comments";

export default function RecipeDetail() {
  const [recipe, setRecipe] = useState<Recipe>();
  const { id } = useParams();
  const api = import.meta.env.VITE_API_URL;

  useEffect(() => {
    const fetchRecipe = async () => {
      try {
        const res = await axios.get(`${api}/recipe/${id}`);
        setRecipe(res.data);
      } catch (error) {
        console.log(error);
      }
    };

    fetchRecipe();
  }, [id, api]);

  return (
    <Container className="py-4">
      <Row>
        <Col md={10} lg={8} xl={6} className="mx-auto">
          <div className="recipe-detail-card" style={{
            background: "white",
            borderRadius: "12px",
            overflow: "hidden",
            boxShadow: "0 2px 16px rgba(0,0,0,0.08)"
          }}>
            <img
              src={recipe?.image}
              alt={recipe?.title}
              style={{ width: "100%", height: "250px", objectFit: "cover" }}
            />

            <div style={{ padding: "1.25rem" }}>
              <h1 style={{ fontSize: "1.5rem", color: "var(--primary-color)", fontWeight: 700 }} className="mb-3">
                {recipe?.title}
              </h1>

              <div
                className="text-muted mb-3 pb-3 border-bottom"
                style={{ fontSize: "0.95rem" }}
                dangerouslySetInnerHTML={{ __html: recipe?.summary || "" }}
              />

              <div className="mb-4">
                <h2 style={{ color: "var(--primary-color)", fontWeight: 600, fontSize: "1.1rem" }} className="mb-2">
                  Ingredienser
                </h2>
                <ul style={{ paddingLeft: "1.25rem", fontSize: "0.95rem" }}>
                  {recipe?.extendedIngredients.map((ing, index) => (
                    <li key={index} className="mb-1">{ing}</li>
                  ))}
                </ul>
              </div>

              <div>
                <h2 style={{ color: "var(--primary-color)", fontWeight: 600, fontSize: "1.1rem" }} className="mb-2">
                  Instruksjoner
                </h2>
                <div
                  style={{ fontSize: "0.95rem" }}
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
