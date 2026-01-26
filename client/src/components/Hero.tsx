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
          <h1 className="hero-title">Oppdag Nye Smaker</h1>
          <p className="hero-subtitle">
            Finn perfekte oppskrifter basert på dine preferanser
          </p>
          <div className="hero-buttons">
            <Button
              variant="primary"
              size="lg"
              className="hero-btn-primary"
              onClick={() => navigate("/recipes")}
            >
              Utforsk oppskrifter
            </Button>
            {!isLoggedIn && (
              <Button
                variant="outline-light"
                size="lg"
                className="hero-btn-secondary"
                onClick={() => navigate("/signup")}
              >
                Registrer deg
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
                title="Søk enkelt"
                description="Finn oppskrifter basert på kalorier og ingredienser"
              />
            </Col>
            <Col md={4}>
              <FeatureCard
                icon="⭐"
                title="Vurder oppskrifter"
                description="Gi stjerner fra 1-5 og del din mening"
              />
            </Col>
            <Col md={4}>
              <FeatureCard
                icon="👥"
                title="Følg andre"
                description="Følg andre brukere og se hva de lager"
              />
            </Col>
          </Row>
        </Container>
      </section>

      {/* Popular Recipes Section */}
      <section className="popular-section">
        <Container>
          <h2 className="section-title">Populære oppskrifter</h2>
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
                      Se oppskrift
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
          <h2 className="cta-title">Klar til å lage noe godt?</h2>
          <p className="cta-subtitle">
            Bli med i fellesskapet og oppdag tusenvis av oppskrifter
          </p>
          <Button
            variant="light"
            size="lg"
            className="cta-btn"
            onClick={() => navigate(isLoggedIn ? "/recipes" : "/signup")}
          >
            {isLoggedIn ? "Finn oppskrifter" : "Kom i gang gratis"}
          </Button>
        </Container>
      </section>
    </>
  );
}
