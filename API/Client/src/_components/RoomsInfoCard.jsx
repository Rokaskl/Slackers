import React from 'react';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';

export class InfoCard extends React.Component{
    render() {
        return (
      
<div className="col-md-10 ml-auto col-xl-6 mr-auto">
  
  {/* Nav tabs */}
  <div className="card">
    <div className="card-header">
      <ul className="nav nav-tabs justify-content-center" role="tablist">
        <li className="nav-item">
          <a className="nav-link active" data-toggle="tab" href="#home" role="tab" aria-selected="true">
            <i className="now-ui-icons media-1_button-play" />
            Start
          </a>
        </li>
        <li className="nav-item">
          <a className="nav-link" data-toggle="tab" href="#profile" role="tab" aria-selected="false">
            <i className="now-ui-icons business_chart-bar-32" />
            Charts
          </a>
        </li>
        <li className="nav-item">
          <a className="nav-link" data-toggle="tab" href="#messages" role="tab" aria-selected="false">
            <i className="now-ui-icons ui-1_email-85" />
            Messages
          </a>
        </li>
        <li className="nav-item">
          <a className="nav-link" data-toggle="tab" href="#settings" role="tab" aria-selected="false">
            <i className="now-ui-icons ui-2_settings-90" />
            Future
          </a>
        </li>
      </ul>
    </div>
    <div className="card-body">
      {/* Tab panes */}
      <div className="tab-content text-center">
        <div className="tab-pane active" id="home" role="tabpanel">
            <p>Easily create room from website or windows based app, send generated key to your team and all what they need to do just paste the key in app</p>
        </div>
        <div className="tab-pane" id="profile" role="tabpanel">
        <p>From our website you can easily get information about your rooms, get time charts (More coming soon)</p>

        </div>
        <div className="tab-pane" id="messages" role="tabpanel">
        <p>In our app each room has messages section where you can live chat with your coworkers and also there is notes sections</p>

        </div>
        <div className="tab-pane" id="settings" role="tabpanel">
        <p>We as a team got a vision to make this platform all in one tool for developers and maybe someone else...</p>

        </div>
      </div>
    </div>
  </div>
</div>

        );
    }
}
