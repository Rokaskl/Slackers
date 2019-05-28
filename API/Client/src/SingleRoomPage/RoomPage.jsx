import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';
import SideBar from '../_components/RoomAdminSideBar.jsx'
import {AdminChart} from '../_components/RoomAdminChart'
import { Image} from 'react-bootstrap';
import DatePicker from "react-datepicker";
import { Navbar, NavItem, NavDropdown, MenuItem, Nav, Form, FormControl , Button,img,Card,Jarallax} from 'react-bootstrap';
import { roomActions } from '../_actions/room.actions.js';
import Chart from 'react-apexcharts'
import { timingSafeEqual } from 'crypto';


 class RoomPage extends React.Component {
  constructor(props) {
    super(props);
    
    this.state = {
      startDate: "2019-05-14",
      endDate: "2019-05-21",
     
      
    };
    this.handleChangeStart = this.handleChangeStart.bind(this);
    this.handleChangeEnd = this.handleChangeEnd.bind(this);
    this.updateTimes = this.updateTimes.bind(this);

    
  }

  handleChangeStart(event) {
    console.log("New  Start Time selected  " )

    this.setState({
      startDate: event.target.value
    });
  }
  handleChangeEnd(event) {
    console.log("New End Time selected  " )
    this.setState({
      endDate: event.target.value
    });
  }
  updateTimes(e,from,to,id){
    e.preventDefault();
    console.log("Loading Chart");
    this.props.dispatch(roomActions.getTimes(from,to,id)) ;
    
    //console.log("Laikai" +times);
   // console.log(times.loading);
   // <AdminChart times = {times}/>


    }
   
  componentDidMount() {
    this.props.dispatch(roomActions.getAll());
    //this.props.dispatch(roomActions.getById(1));

    
   }
    render() {
    
      const { user,rooms,room,times} = this.props;
     
      console.log("Id from Props");

      console.log(this.props.match.params.id);

      console.log("State");

      console.log(this.state);
      
      var LoadingChart = false;
      
     
        
      
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
                                      {rooms.items.map((temproom, index) =>
                                              <li key={temproom.roomId}>
                                                
                                                
                                              </li>
                                              
                                              
                                          )}
                                          
                                      </ul>
                                  }
                           </div>
                          </div>
                      </div>
                 </a>
                </li>
               {/* <li className="nav-item">
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
                                </li>*/}
              </ul>
              
            </div>

          </nav>
    <main role="main" className="col-md-9 ml-sm-auto col-lg-10 px-4"><div className="chartjs-size-monitor" style={{position: 'absolute', left: '0px', top: '0px', right: '0px', bottom: '0px', overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div className="chartjs-size-monitor-expand" style={{position: 'absolute', left: 0, top: 0, right: 0, bottom: 0, overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div style={{position: 'absolute', width: '1000000px', height: '1000000px', left: 0, top: 0}} /></div><div className="chartjs-size-monitor-shrink" style={{position: 'absolute', left: 0, top: 0, right: 0, bottom: 0, overflow: 'hidden', pointerEvents: 'none', visibility: 'hidden', zIndex: -1}}><div style={{position: 'absolute', width: '200%', height: '200%', left: 0, top: 0}} /></div></div>
    <div className="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <h1 className="h2">Times chart</h1>
        <div className="btn-toolbar mb-2 mb-md-0">
         <div className="btn-group mr-2">
          <form class="form-inline">
             <div class="col-md-6">
					          <h4>From</h4>
                    <input type="text" class="form-control date-picker" value={this.state.startDate} onChange={this.handleChangeStart} data-date-format="yyyy-mm-dd" data-datepicker-color="secondary"/>
              </div>
             <div class="col-md-6">
					        <h4>To</h4>
                   <input type="text" class="form-control date-picker"  value={this.state.endDate} onChange={this.handleChangeEnd}  data-date-format="yyyy-mm-dd" data-datepicker-color="secondary"/>
              </div>
                <button type="button" className="btn btn-primary" onClick={(e) =>this.updateTimes(
                  e,
                  this.state.startDate,
                  this.state.endDate,
                  this.props.match.params.id
                  )}>Update</button>
           </form>
        </div>
       </div>
     </div>
                        <h3>Room Name...</h3>
                       
                          
                        {!times.loading  ?   (
                           LoadingChart = true,
                          <AdminChart times = {times} />
                        ) : (
                          <h3>Admin chart data is loading...</h3> 
                        )}
                       {/* {times.loading && <em>Loading room times...</em>}
                              {time.error && <span className="text-danger">ERROR: {rooms.error}</span>}
                              {times.items &&
                                       <ul className="nav flex-column">
                                      {times.items.map((time, index) =>
                                              <li key={time.name}>
                                                
                                                
                                              </li>
                                              
                                              
                                          )}
                                          
                                      </ul>
                              }
                            */}
    
                          
                         
   
    {/*<form class="form-inline">
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
                              {times.loading && <em>Loading rooms...</em>}
                              {times.error && <span className="text-danger">ERROR: {times.error}</span>}
                              {times.items &&
                                       <ul className="nav flex-column">
                                      {times.items.map((time, index) =>
                                              <li key={time.Id}>
                                                
                                          
                                                  <p>{time.Times}</p>
                                               
                                              </li>
                                              
                                          )}
                                          
                                      </ul>
                                  }
                          
                     
               
       </form>
                                */}
  
    
    
    
     
    </main>
  </div>
</div>


        );
    }
}


function mapStateToProps(state) {
  const { rooms, authentication,room,times } = state;
  const { user } = authentication;



  return {
      user,
      rooms,
      room,
      times
  };
}

const connectedRoomPage = connect(mapStateToProps)(RoomPage);
export { connectedRoomPage as RoomPage };

