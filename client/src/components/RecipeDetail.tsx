import axios from "axios";
import { useEffect, useState } from "react";
import {  Col, Container, Row } from "react-bootstrap";
import { useParams } from "react-router-dom";

export default function RecipeDetial() {
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
<Container className="my-5">
  <Row>
    <Col lg={10} xl={8} className="mx-auto">
      <img 
        src={recipe?.image} 
        alt={recipe?.title}
        className="img-fluid rounded shadow-sm mb-4"
        style={{ width: "100%", height: "400px", objectFit: "cover" }}
      />

      <h1 className="display-4 fw-bold mb-4">
        {recipe?.title}
      </h1>

      <div 
        className="lead text-muted mb-4 pb-4 border-bottom"
        dangerouslySetInnerHTML={{ __html: recipe?.summary || "" }} 
      />

      <div className="mb-5">
        <h2 className="h3 fw-semibold mb-3">Ingredienser</h2>
        <ul className="fs-5 lh-lg">
          {recipe?.extendedIngredients.map((ing, index) => (
            <li key={index} className="mb-2">{ing}</li>
          ))}
        </ul>
      </div>

      <div className="mb-5">
        <h2 className="h3 fw-semibold mb-3">Instruksjoner</h2>
        <div
          className="fs-5 lh-lg"
          dangerouslySetInnerHTML={{ __html: recipe?.instructions || "" }}
        />
      </div>
    </Col>
  </Row>
</Container>
  );
}
