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
        <h5 className="description">We are "Wbies". Company of young motivated computer science students at University of Kaunas Technology </h5>
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
          <h3>So what does the "SLACKERS" is for?</h3>
          <p>The Arctic Ocean freezes every winter and much of the sea-ice
            then thaws every summer, and that process will continue whatever happens with climate change. Even if the Arctic continues to be one of the fastest-warming regions of the world, it will always be plunged into bitterly cold polar dark every winter. And year-by-year, for all kinds of natural reasons, there’s huge variety of the state of the ice.
          </p>
          <p>
            For a start, it does not automatically do the job for your, but it definetly helps to track your working time.
          </p>
          <p>The Arctic Ocean freezes every winter and much of the sea-ice
            then thaws every summer, and that process will continue whatever happens with climate change. Even if the Arctic continues to be one of the fastest-warming regions of the world, it will always be plunged into bitterly cold polar dark every winter. And year-by-year, for all kinds of natural reasons, there’s huge variety of the state of the ice.
          </p>
        </div>
      </div>
    </div>
  </div>
</div>

        );
    }
}
