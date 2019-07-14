import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';

export class Section2 extends React.Component{
    render() {
        return (
      

<div className="section section-about-us">
  <div className="container">
    <div className="row">
      <div className="col-md-8 ml-auto mr-auto text-center">
        <h2 className="title">Who we are?</h2>
        <h5 className="description">We are "Wbies". Company of young motivated computer science students at Kaunas University of Technology </h5>
      </div>
    </div>
    <div className="separator separator-primary" />
    <div className="section-story-overview">
      <div className="row">
        <div className="col-md-6">
          <div className="image-container image-left" style={{backgroundImage: 'url("../images/About1.jpg")'}}>
            {/* First image on the left side */}
            <p className="blockquote blockquote-primary">"Programa, manau, tikrai verta demesio.Labai grazus UI svetaines, akivaizdziai Front-End guy is the coolest one! "
              <br />
              <br />
              <small>-Rokas</small>
            </p>
          </div>
          {/* Second image on the left side of the article */}
          <div className="image-container" style={{backgroundImage: 'url("../images/About3.jpeg")'}} />
        </div>
        <div className="col-md-5">
          {/* First image on the right side, above the article */}
          <div className="image-container image-right" style={{backgroundImage: 'url("../images/About2.jpg")'}} />
          <h3>So what does the "SLACKERS!" is for?</h3>
          <p> 
            Slackers platform build for small teams and especially for their manager.
          </p>
          <p> 
            In this app you can easily find who is working at the moment, get statiscs who worked the most hours etc...
          </p>
        </div>
      </div>
    </div>
  </div>
</div>

        );
    }
}
