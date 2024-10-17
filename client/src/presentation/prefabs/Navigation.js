import 'bootstrap/dist/css/bootstrap.min.css';
import { useContext } from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import NavDropdown from 'react-bootstrap/NavDropdown';
import { RootContext } from '../domain/context/RootContextProvider';

export default function TopNavigation (){
  const context = useContext(RootContext);

    return(
        <>
    <Navbar variant="body" expand="lg" className="bg-body-tertiary" style={{marginLeft: 0, paddingLeft: 0, height: context.constants.topNavBarHeight}}>
      <Container>
        <Navbar.Brand href="#home">BMK Place</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            <Nav.Link href="/">Home</Nav.Link>
            <Nav.Link href="canvases">Canvases</Nav.Link>
            {/* <NavDropdown title="Settings" id="basic-nav-dropdown">
              <NavDropdown.Item href="#action/3.1">Action</NavDropdown.Item>
              <NavDropdown.Item href="#action/3.2">
                Another action
              </NavDropdown.Item>
              <NavDropdown.Item href="#action/3.3">Something</NavDropdown.Item>
              <NavDropdown.Divider />
              <NavDropdown.Item href="#action/3.4">
                Separated link
              </NavDropdown.Item>
            </NavDropdown> */}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
        </>
    );
}