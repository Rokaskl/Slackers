import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import SideBar from '../_components/RoomAdminSideBar.jsx'
import {AdminChart} from '../_components/RoomAdminChart'
import { Image} from 'react-bootstrap';
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';

import { roomActions } from '../_actions/room.actions.js';

 class RoomPage extends React.Component {
  
  componentDidMount() {
    this.props.dispatch(roomActions.getAll());
    
   }
    render() {
      const { user,rooms} = this.props;
      var currentRoom = null;
      
     
        
      
        return (
      
          <div className="container-fluid">
  <div className="row">
  <nav className="col-md-2 d-none d-md-block bg-light sidebar">
            <div className="sidebar-sticky">
              <ul className="nav flex-column">
                <li className="nav-item">
                  <a className="nav-link active" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-home"><path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" /><polyline points="9 22 9 12 15 12 15 22" /></svg>
                    Dashboard <span className="sr-only">(current)</span>
                  </a>
                </li>
                <li className="nav-item">
                <a className="nav-link active" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-home"><path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" /><polyline points="9 22 9 12 15 12 15 22" /></svg>
                    <div class="panel-group">
                  <div class="panel panel-default">
                    <div class="panel-heading">
                      <a class="panel-title">
                        <a data-toggle="collapse" href="#collapse1">Rooms</a>
                      </a>
                        </div>
                          <div id="collapse1" class="panel-collapse collapse">
                              {rooms.loading && <em>Loading rooms...</em>}
                              {rooms.error && <span className="text-danger">ERROR: {rooms.error}</span>}
                              {rooms.items &&
                                       <ul className="nav flex-column">
                                      {rooms.items.map((room, index) =>
                                              <li key={room.roomId}>
                                                <a className="avatar" href="#">
                                                  <p>{room.roomName}</p>
                                                </a>
                                              </li>
                                          )}
                                      </ul>
                                  }
                           </div>
                          </div>
                      </div>
                 </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-shopping-cart"><circle cx={9} cy={21} r={1} /><circle cx={20} cy={21} r={1} /><path d="M1 1h4l2.68 13.39a2 2 0 0 0 2 1.61h9.72a2 2 0 0 0 2-1.61L23 6H6" /></svg>
                    Products
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-users"><path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx={9} cy={7} r={4} /><path d="M23 21v-2a4 4 0 0 0-3-3.87" /><path d="M16 3.13a4 4 0 0 1 0 7.75" /></svg>
                    Customers
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-bar-chart-2"><line x1={18} y1={20} x2={18} y2={10} /><line x1={12} y1={20} x2={12} y2={4} /><line x1={6} y1={20} x2={6} y2={14} /></svg>
                    Reports
                  </a>
                </li>
                <li className="nav-item">
                  <a className="nav-link" href="#">
                    <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-layers"><polygon points="12 2 2 7 12 12 22 7 12 2" /><polyline points="2 17 12 22 22 17" /><polyline points="2 12 12 17 22 12" /></svg>
                    Integrations
                  </a>
                </li>
              </ul>
              
            </div>

          </nav>
    <main role="main" className="col-md-9 ml-sm-auto col-lg-10 px-4"><div className="chartjs-size-monitor" style={{position: 'absolute', left: '0px', top: '0px', right: '0px', bottom: '0px', overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div className="chartjs-size-monitor-expand" style={{position: 'absolute', left: 0, top: 0, right: 0, bottom: 0, overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div style={{position: 'absolute', width: '1000000px', height: '1000000px', left: 0, top: 0}} /></div><div className="chartjs-size-monitor-shrink" style={{position: 'absolute', left: 0, top: 0, right: 0, bottom: 0, overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div style={{position: 'absolute', width: '200%', height: '200%', left: 0, top: 0}} /></div></div>
    <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h1 className="h2">Dashboard</h1>
        <div className="btn-toolbar mb-2 mb-md-0">
          <div className="btn-group mr-2">
            <button className="btn btn-sm btn-outline-secondary">Share</button>
            <button className="btn btn-sm btn-outline-secondary">Export</button>
          </div>
          <button className="btn btn-sm btn-outline-secondary dropdown-toggle">
            <svg xmlns="http://www.w3.org/2000/svg" width={24} height={24} viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth={2} strokeLinecap="round" strokeLinejoin="round" className="feather feather-calendar"><rect x={3} y={4} width={18} height={18} rx={2} ry={2} /><line x1={16} y1={2} x2={16} y2={6} /><line x1={8} y1={2} x2={8} y2={6} /><line x1={3} y1={10} x2={21} y2={10} /></svg>
            This week
          </button>
        </div>
      </div>
    <h3>Room Name...</h3>
   
    <form class="form-inline">
          <a className="avatar" href="#">
            <Image src=".//..//images/avatar1.jpg"   roundedCircle width={50} height={50}  />
            <p>Rokas1</p>
          </a>
          <a className="avatar" href="#">
            <Image src=".//..//images/avatar2.jpg"   roundedCircle width={50} height={50}  />
            <p>Rokas2</p>
          </a>
          <a className="avatar" href="#">
            <Image src=".//..//images/avatar3.jpg"   roundedCircle width={50} height={50}  />
            <p>Rokas3</p>
          </a>
          <a className="avatar" href="#">
            <Image src=".//..//images/avatar4.jpg"   roundedCircle width={50} height={50}  />
            <p>Rokas4</p>
          </a>
       </form>
  
    
    <AdminChart />
    
      
      {/*<h2>Section title</h2>
      <div className="table-responsive">
        <table className="table table-striped table-sm">
          <thead>
            <tr>
              <th>#</th>
              <th>Header</th>
              <th>Header</th>
              <th>Header</th>
              <th>Header</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td>1,001</td>
              <td>Lorem</td>
              <td>ipsum</td>
              <td>dolor</td>
              <td>sit</td>
            </tr>
            <tr>
              <td>1,002</td>
              <td>amet</td>
              <td>consectetur</td>
              <td>adipiscing</td>
              <td>elit</td>
            </tr>
            <tr>
              <td>1,003</td>
              <td>Integer</td>
              <td>nec</td>
              <td>odio</td>
              <td>Praesent</td>
            </tr>
            <tr>
              <td>1,003</td>
              <td>libero</td>
              <td>Sed</td>
              <td>cursus</td>
              <td>ante</td>
            </tr>
            <tr>
              <td>1,004</td>
              <td>dapibus</td>
              <td>diam</td>
              <td>Sed</td>
              <td>nisi</td>
            </tr>
            <tr>
              <td>1,005</td>
              <td>Nulla</td>
              <td>quis</td>
              <td>sem</td>
              <td>at</td>
            </tr>
            <tr>
              <td>1,006</td>
              <td>nibh</td>
              <td>elementum</td>
              <td>imperdiet</td>
              <td>Duis</td>
            </tr>
            <tr>
              <td>1,007</td>
              <td>sagittis</td>
              <td>ipsum</td>
              <td>Praesent</td>
              <td>mauris</td>
            </tr>
            <tr>
              <td>1,008</td>
              <td>Fusce</td>
              <td>nec</td>
              <td>tellus</td>
              <td>sed</td>
            </tr>
            <tr>
              <td>1,009</td>
              <td>augue</td>
              <td>semper</td>
              <td>porta</td>
              <td>Mauris</td>
            </tr>
            <tr>
              <td>1,010</td>
              <td>massa</td>
              <td>Vestibulum</td>
              <td>lacinia</td>
              <td>arcu</td>
            </tr>
            <tr>
              <td>1,011</td>
              <td>eget</td>
              <td>nulla</td>
              <td>Class</td>
              <td>aptent</td>
            </tr>
            <tr>
              <td>1,012</td>
              <td>taciti</td>
              <td>sociosqu</td>
              <td>ad</td>
              <td>litora</td>
            </tr>
            <tr>
              <td>1,013</td>
              <td>torquent</td>
              <td>per</td>
              <td>conubia</td>
              <td>nostra</td>
            </tr>
            <tr>
              <td>1,014</td>
              <td>per</td>
              <td>inceptos</td>
              <td>himenaeos</td>
              <td>Curabitur</td>
            </tr>
            <tr>
              <td>1,015</td>
              <td>sodales</td>
              <td>ligula</td>
              <td>in</td>
              <td>libero</td>
            </tr>
          </tbody>
        </table>
      </div>*/}  
    </main>
  </div>
</div>


        );
    }
}

function mapStateToProps(state) {
  const { rooms, authentication } = state;
  const { user } = authentication;



  return {
      user,
      rooms
  };
}

const connectedRoomPage = connect(mapStateToProps)(RoomPage);
export { connectedRoomPage as RoomPage };

