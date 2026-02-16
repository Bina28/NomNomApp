import axios from "axios";
import { useEffect, useState } from "react";
import { Button, Card, Col, Container, Row } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

type Recipe = {
  id: number;
  title: string;
  image: string;
};

type FeatureCardProps = {
  icon: string;
  title: string;
  description: string;
};

function FeatureCard({ icon, title, description }: FeatureCardProps) {
  return (
    <div className="feature-card">
      <div className="feature-icon">{icon}</div>
      <h3 className="feature-title">{title}</h3>
      <p className="feature-description">{description}</p>
    </div>
  );
}

export default function Hero() {
  const [popularRecipes, setPopularRecipes] = useState<Recipe[]>([]);
  const navigate = useNavigate();
  const { isLoggedIn } = useAuth();
  const api = import.meta.env.VITE_API_URL;

  useEffect(() => {
    axios
      .get(`${api}/recipe/search?calories=300&number=4`)
      .then((res) => setPopularRecipes(res.data));
  }, [api]);

  return (
    <>
      {/* Hero Section */}
      <section className="hero-section">
        <Container>
          <h1 className="hero-title">Discover New Flavors</h1>
          <p className="hero-subtitle">
            Find the perfect recipes based on your preferences
          </p>
          <div className="hero-buttons">
            <Button
              variant="primary"
              size="lg"
              className="hero-btn-primary"
              onClick={() => navigate("/recipes")}
            >
              Explore Recipes
            </Button>
            {!isLoggedIn && (
              <Button
                variant="outline-light"
                size="lg"
                className="hero-btn-secondary"
                onClick={() => navigate("/signup")}
              >
                Sign Up
              </Button>
            )}
          </div>
        </Container>
      </section>

      {/* Features Section */}
      <section className="features-section">
        <Container>
          <Row className="g-4">
            <Col md={4}>
              <FeatureCard
                icon="🔍"
                title="Easy Search"
                description="Find recipes based on calories and ingredients"
              />
            </Col>
            <Col md={4}>
              <FeatureCard
                icon="⭐"
                title="Rate Recipes"
                description="Give 1-5 stars and share your opinion"
              />
            </Col>
            <Col md={4}>
              <FeatureCard
                icon="👥"
                title="Follow Others"
                description="Follow other users and see what they make"
              />
            </Col>
          </Row>
        </Container>
      </section>

      {/* Popular Recipes Section */}
      <section className="popular-section">
        <Container>
          <h2 className="section-title">Popular Recipes</h2>
          <Row className="g-4">
            {popularRecipes.map((recipe) => (
              <Col xs={6} md={3} key={recipe.id}>
                <Card className="recipe-card h-100">
                  <div style={{ overflow: "hidden" }}>
                    <Card.Img variant="top" src={recipe.image} />
                  </div>
                  <Card.Body className="d-flex flex-column">
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
              </Col>
            ))}
          </Row>
        </Container>
      </section>

      {/* CTA Section */}
      <section className="cta-section">
        <Container>
          <h2 className="cta-title">Ready to cook something delicious?</h2>
          <p className="cta-subtitle">
            Join the community and discover thousands of recipes
          </p>
          <Button
            variant="light"
            size="lg"
            className="cta-btn"
            onClick={() => navigate(isLoggedIn ? "/recipes" : "/signup")}
          >
            {isLoggedIn ? "Find Recipes" : "Get Started for Free"}
          </Button>
        </Container>
      </section>
    </>
  );
}
