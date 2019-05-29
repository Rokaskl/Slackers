import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button, DropdownButton,Image} from 'react-bootstrap';
import { connect } from 'react-redux';

class NavigationBar extends React.Component{
    render() {
      const { authentication  } = this.props;
      
        return (
        <div>
          <nav className="navbar navbar-expand-lg bg-primary fixed-top navbar-transparent" color-on-scroll={40}>
              <Navbar.Brand href="#home">Slackers</Navbar.Brand>
                <Nav className="mr-auto">
                  <Nav.Link href="./">Home</Nav.Link>
                  <Nav.Link href="https://github.com/Rokaskl/Slackers">GIT</Nav.Link>
                  
                  <Nav.Link href="./rooms">Rooms</Nav.Link>
                </Nav>
                <ButtonsOrProfile authentication={this.props.authentication}/>
            </nav>
          </div>
        );
    }
    
}
function mapStateToProps(state) {
  const { authentication  } = state;

  return {
      authentication
  };
}
function ButtonsOrProfile(props)
{
  
  if(props.authentication.loggedIn == undefined || props.authentication.loggedIn ==false)
   {
        return(
        <div>
          
        <a href="./login" className="btn btn-primary" role="button" aria-pressed="true">Login</a>
        <a href="./register" className="btn btn-secondary" role="button" aria-pressed="true">Register</a>
        </div>)
      }
      else
      {
        var style = {
          color: 'white',
          padding: "5px",
          fontWeight: "550"
        };
    
        return(
          <form className="form-inline">
          Signed in as:  
          <div style={style}>{'  '}{props.authentication.user.username}</div>  
          <a className="avatar" href="./profilePage">
            <Image src=".//..//images/avatar.png"   roundedCircle width={50} height={50}  />
            <a href="./login" className="btn btn-primary" role="button" aria-pressed="true">Logout</a>
          </a>
       </form>
       )

      }
    }


               
             
  
const connectedNavBar = connect(mapStateToProps)(NavigationBar);
export { connectedNavBar as NavigationBar };
