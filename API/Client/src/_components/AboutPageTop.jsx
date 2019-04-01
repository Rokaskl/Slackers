import React from 'react';
import {Jumbotrone, Card, Button, Video, Section} from 'react-bootstrap';

export class AboutPageTop extends React.Component{
    render() {
        return (
        <div>
          <div className="jumbotron jumbotron-fluid">
          <video className="video-background" preload="true" muted="true" autoPlay="true" loop="true">
              <source src="..//..//images/Network.mp4" type="video/mp4" />
          </video>
          <div className="container">

          <div className="text-white text-center py-5 px-4 my-5">
            <div>
                <h1>Start Now!</h1>
              
            </div>
            <button type="button" class="btn btn-light">REGISTER</button>
            </div>
          </div>
        </div>
      </div>



        );
    }
}
