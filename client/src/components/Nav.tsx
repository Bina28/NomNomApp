import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

function BasicExample() {
  const { isLoggedIn, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    await logout();
    navigate('/');
  };

  return (
    <Navbar expand="lg" className="navbar-custom" data-bs-theme="dark">
      <Container>
        <Navbar.Brand as={Link} to="/">NomNom</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link as={Link} to="/recipes">Oppskrifter</Nav.Link>
            <Nav.Link as={Link} to="/create-recipe">Lag oppskrift</Nav.Link>
          </Nav>
          <Nav>
            {isLoggedIn ? (
              <NavDropdown title={user?.userName || 'Konto'} id="basic-nav-dropdown" align="end">
                <NavDropdown.Item as={Link} to="/userPage">Min profil</NavDropdown.Item>
                <NavDropdown.Item href="#action/3.3">Mine oppskrifter</NavDropdown.Item>
                <NavDropdown.Item href="#action/3.3">Favoritter</NavDropdown.Item>
                <NavDropdown.Divider />
                <NavDropdown.Item onClick={handleLogout}>Logg ut</NavDropdown.Item>
              </NavDropdown>
            ) : (
              <NavDropdown title="Konto" id="basic-nav-dropdown" align="end">
                <NavDropdown.Item as={Link} to="/login">Logg inn</NavDropdown.Item>
                <NavDropdown.Item as={Link} to="/signUp">Registrer deg</NavDropdown.Item>
              </NavDropdown>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
}

export default BasicExample;