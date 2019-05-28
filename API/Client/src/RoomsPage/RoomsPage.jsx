import React from 'react';
import { connect } from 'react-redux';
import { RoomPage } from '../SingleRoomPage';
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import { PrivateRoute } from '../_components';

import { roomActions } from '../_actions/room.actions.js';
import RoomCard from '../_components/RoomCard.jsx';
import AddRoom from '../_components/AddRoom.jsx';
import {button,btn} from 'react-bootstrap';

class RoomsPage extends React.Component {
    constructor(props) {
        super(props);
        
        this.state = {
         newRoom: {
            roomName: ''
            },
            submitted: false
        };

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);

    }
    componentDidMount() {
     this.props.dispatch(roomActions.getAll());
     
    }

    handleDeleteroom(id) {
        return (e) => this.props.dispatch(roomActions.delete(id));
        
    }
   
    handleChange(e) {
        const { name, value } = e.target;
        const {newRoom} =this.state;
        this.setState({
            newRoom: {
                ...newRoom,
                [name]: value

            }
        });
    }
    handleSubmit(event) {
        event.preventDefault();

        this.setState({ submitted: true });
        const { newRoom } = this.state;
        const { dispatch } = this.props;
        if (newRoom.roomName) {
            dispatch(roomActions.register(newRoom));
        }
    }
    
   
    render() {
        
        const { user, rooms} = this.props;
        const {newRoom} =this.state;
            return (
            <div>
                 
                <h1>Rooms page</h1>
                 {/* Button trigger modal */}
                 <button type="button" className="btn btn-primary btn-icon btn-round btn-lg" data-toggle="modal" data-target="#exampleModal">
                      <i className="now-ui-icons ui-1_simple-add"></i>
                 </button>
                {rooms.loading && <em>Loading rooms...</em>}
                {rooms.error && <span className="text-danger">ERROR: {rooms.error}</span>}
                {rooms.items &&
                   <div>
                        
                        {rooms.items.map((room, index) =>
                            <div key={room.roomId}>
                             <div className="card" style={{width: '20rem'}}>
                                {RoomCard({room})}
                            
                                {   
                                    room.deleting ? <em> - Deleting...</em>
                                    : room.deleteError ? <span className="text-danger"> - ERROR: {room.deleteError}</span>
                                    :   <span> 
                                        
                                         <a href={`/${room.roomId}`} className="btn btn-secondary" role="button" aria-pressed="true">Room Info</a>
                                         <button type="button" className="btn btn-primary" onClick={this.handleDeleteroom(room.roomId)}>Delete</button>
                                        
                                        </span>
                                }
                                </div>
                             </div>
                        )}
                   </div>
                }
                  
                   
                    {/* Create New Room Modal */}
                 <div className="modal fade" id="exampleModal" tabIndex={-1} role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">Add New Room</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">×</span>
                            </button>
                            </div>
                            <div className="modal-body">
                            <label form="roomName">Room Name</label>
                             <input type="text" className="form-control" name="roomName" value={newRoom.roomName} onChange={this.handleChange} placeholder="Enter New Room Name" />
                             </div>
                            <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-dismiss="modal">Close</button>
                            <button type="button" className="btn btn-primary" onClick={this.handleSubmit}>Create</button>
                            </div>
                        </div>
                    </div>
                 </div>
                 {/*Get room guid modal    

                 <div className="modal fade" id="exampleModal2" tabIndex={-1} role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                    <div className="modal-dialog" role="document">
                        <div className="modal-content">
                            <div className="modal-header">
                            <h5 className="modal-title" id="exampleModalLabel">{tempGuid}</h5>
                            <button type="button" className="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">×</span>
                            </button>
                            </div>
                            <div className="modal-body">
                            <label form="roomName">Room GUID</label>

                             </div>
                            <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-dismiss="modal">Close</button>
                            </div>
                        </div>
                    </div>
                 </div>
                 */}

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

const connectedRoomsPage = connect(mapStateToProps)(RoomsPage);
export { connectedRoomsPage as RoomsPage };
