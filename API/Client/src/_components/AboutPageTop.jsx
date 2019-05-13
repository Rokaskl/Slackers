import React from 'react';
import {Jumbotrone, Card, Button, Video, Section, ol,li,img} from 'react-bootstrap';

export class AboutPageTop extends React.Component{
    render() {
        return (
          <div className="page-header page-header-small">
          <div className="page-header-image" data-parallax="true" style={{backgroundImage: 'url("../images/bck.jpg")', transform: 'translate3d(0px, 0px, 0px)'}}>
          </div>
          <div className="content-center">
            <div className="container">
              <h1 className="title">Join OUR comunity NOW!</h1>
              <button className="btn btn-primary btn-round btn-lg">Join NOW!</button>
              <div className="text-center">
                                      

                <a href="#pablo" className="btn btn-primary btn-icon btn-round">
                  <i className="fab fa-facebook-square" />
                </a>
                <a href="#pablo" className="btn btn-primary btn-icon btn-round">
                  <i className="fab fa-twitter" />
                </a>
                <a href="#pablo" className="btn btn-primary btn-icon btn-round">
                  <i className="fab fa-google-plus" />
                </a>
              </div>
            </div>
          </div>
        </div>
        
        );
    }
}
