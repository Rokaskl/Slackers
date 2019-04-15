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
        <img src="../images/avatar.png" alt />
      </div>
      <h3 className="title">Ryan Scheinder</h3>
      <p className="category">Photographer</p>
      <div className="content">
        <div className="social-description">
          <h2>26</h2>
          <p>Comments</p>
        </div>
        <div className="social-description">
          <h2>26</h2>
          <p>Comments</p>
        </div>
        <div className="social-description">
          <h2>48</h2>
          <p>Bookmarks</p>
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
      <h5 className="description">An artist of considerable range, Ryan — the name taken by Melbourne-raised, Brooklyn-based Nick Murphy — writes, performs and records all of his own music, giving it a warm, intimate feel with a solid groove structure. An artist of considerable range.</h5>
      <div className="row">
        <div className="col-md-6 ml-auto mr-auto">
          <h4 className="title text-center">My Portfolio</h4>
          <div className="nav-align-center">
            <ul className="nav nav-pills nav-pills-primary nav-pills-just-icons" role="tablist">
              <li className="nav-item">
                <a className="nav-link" data-toggle="tab" href="#profile" role="tablist">
                  <i className="now-ui-icons design_image" />
                </a>
              </li>
              <li className="nav-item">
                <a className="nav-link active" data-toggle="tab" href="#home" role="tablist">
                  <i className="now-ui-icons location_world" />
                </a>
              </li>
              <li className="nav-item">
                <a className="nav-link" data-toggle="tab" href="#messages" role="tablist">
                  <i className="now-ui-icons sport_user-run" />
                </a>
              </li>
            </ul>
          </div>
        </div>
        {/* Tab panes */}
        <div className="tab-content gallery">
          <div className="tab-pane active" id="home" role="tabpanel">
            <div className="col-md-10 ml-auto mr-auto">
              <div className="row collections">
                <div className="col-md-6">
                  <img src="../assets/img/bg1.jpg" alt className="img-raised" />
                  <img src="../assets/img/bg3.jpg" alt className="img-raised" />
                </div>
                <div className="col-md-6">
                  <img src="../assets/img/bg8.jpg" alt className="img-raised" />
                  <img src="../assets/img/bg7.jpg" alt className="img-raised" />
                </div>
              </div>
            </div>
          </div>
          <div className="tab-pane" id="profile" role="tabpanel">
            <div className="col-md-10 ml-auto mr-auto">
              <div className="row collections">
                <div className="col-md-6">
                  <img src="../assets/img/bg6.jpg" className="img-raised" />
                  <img src="../assets/img/bg11.jpg" alt className="img-raised" />
                </div>
                <div className="col-md-6">
                  <img src="../assets/img/bg7.jpg" alt className="img-raised" />
                  <img src="../assets/img/bg8.jpg" alt className="img-raised" />
                </div>
              </div>
            </div>
          </div>
          <div className="tab-pane" id="messages" role="tabpanel">
            <div className="col-md-10 ml-auto mr-auto">
              <div className="row collections">
                <div className="col-md-6">
                  <img src="../assets/img/bg3.jpg" alt className="img-raised" />
                  <img src="../assets/img/bg8.jpg" alt className="img-raised" />
                </div>
                <div className="col-md-6">
                  <img src="../assets/img/bg7.jpg" alt className="img-raised" />
                  <img src="../assets/img/bg6.jpg" className="img-raised" />
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
</div>
        
        

        );
    }
}

