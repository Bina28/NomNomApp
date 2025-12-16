import axios from "axios";
import { useEffect, useState } from "react";
import { Button, Card } from "react-bootstrap";
import { useParams } from "react-router-dom";

export default function RecipeDetial() {
  const [recipe, setRecipe] = useState<Recipe>();
  const { id } = useParams();
  const api = import.meta.env.VITE_API_URL;

  useEffect(() => {
    axios.get(`${api}/recipe/${id}`).then((res) => setRecipe(res.data));
  }, [id]);

  return (
    <Card style={{ width: "18rem" }}>
      <Card.Img variant="top" src={recipe?.image} />
      <Card.Body>
        <Card.Title>{recipe?.title}</Card.Title>
        <Card.Text><div dangerouslySetInnerHTML={{ __html: recipe?.instructions || "" }} /></Card.Text>
        <div dangerouslySetInnerHTML={{ __html: recipe?.summary || "" }} />
        <Card.Text>
          <ul>
            {recipe?.extendedIngredients.map((ing, index) => (
              <li key={index}>{ing}</li>
            ))}
          </ul>
        </Card.Text>
        <Button variant="primary">Go somewhere</Button>
      </Card.Body>
    </Card>
  );
}
