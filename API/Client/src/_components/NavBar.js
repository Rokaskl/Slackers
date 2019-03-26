import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button} from 'react-bootstrap';

export class NavigationBar extends React.Component{
    render() {
        return (
          <nav class="navbar sticky-top navbar-light bg-light">
              <Navbar.Brand href="#home">Slackers</Navbar.Brand>
                <Nav className="mr-auto">
                  <Nav.Link href="./home">Home</Nav.Link>
                  <Nav.Link href="https://github.com/Rokaskl/Slackers">GIT</Nav.Link>
                    <Nav.Link href="./about">About</Nav.Link>
                </Nav>
                <Form inline>
                <button type="button" className="btn btn-outline-primary">Login</button>

                <button type="button" className="btn btn-outline-secondary">Register</button>

                </Form>
            </nav>

        );
    }
}
