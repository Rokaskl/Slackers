import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { userActions } from '../_actions';

import { roomActions } from '../_actions/room.actions.js';
import { Table,label} from 'react-bootstrap';

export class ProfilePage extends React.Component {
  
   

    render() {
        return (
          <div>


  <div className="wrapper">
  <div className="page-header clear-filter" filter-color="orange">
    <div className="page-header-image" data-parallax="true" style={{backgroundImage: 'url("../assets/img/bg5.jpg")'}}>
    </div>
    <div className="container">
      <div className="photo-container">
      <img src="../images/avatar.png" alt="Circle Image" className="rounded-circle" />
      </div>
      <h3 className="title">Rokas Kliucinskas</h3>
      <p className="category">Full-Stack DEV</p>
      <div className="content">
        <div className="social-description">
          <h2>2</h2>
          <p>Rooms Created</p>
        </div>
        <div className="social-description">
          <h2>7</h2>
          <p>Rooms user</p>
        </div>
        <div className="social-description">
          <h2>485</h2>
          <p>Total work hours</p>
        </div>
      </div>
    </div>
  </div>
  <div className="section">
    <div className="container">
      <div className="button-container">
        <a href="#button" className="btn btn-primary btn-round btn-lg">Follow</a>
        <a href="#button" className="btn btn-default btn-round btn-lg btn-icon" rel="tooltip" title="Follow me on Twitter">
          <i className="fab fa-twitter" />
        </a>
        <a href="#button" className="btn btn-default btn-round btn-lg btn-icon" rel="tooltip" title="Follow me on Instagram">
          <i className="fab fa-instagram" />
        </a>
      </div>
      <h3 className="title">About me</h3>
      <h5 className="description">Full stack developer from Lithuania, Kaunas. </h5>
      <div className="row">
        <div className="col-md-6 ml-auto mr-auto">
        </div>
      </div>
    </div>
  </div>
</div>
</div>
        
        

        );
    }
}

