import React from 'react';
import { connect } from 'react-redux';

import { roomActions } from '../_actions/room.actions.js';
import RoomCard from '../_components/RoomCard.jsx';
import {button,btn} from 'react-bootstrap';

class RoomsPage extends React.Component {
    componentDidMount() {
     this.props.dispatch(roomActions.getAll());
     
    }

    handleDeleteroom(id) {
        return (e) => this.props.dispatch(roomActions.delete(id));
        
    }
   
    render() {
        const { user, rooms} = this.props;

            return (
            <div>
                <h1>Rooms page</h1>
                {rooms.loading && <em>Loading rooms...</em>}
                {rooms.error && <span className="text-danger">ERROR: {rooms.error}</span>}
                {rooms.items &&
                        <div className="row">
                        
                        <div className="card text-center">
                        {rooms.items.map((room, index) =>
                            <div key={room.roomId}>
                                {RoomCard({room})}
                                <div className="card-footer">
                                {   
                                    room.deleting ? <em> - Deleting...</em>
                                    : room.deleteError ? <span className="text-danger"> - ERROR: {room.deleteError}</span>
                                    :   <span> 
                                         <a href="/room" className="btn btn-secondary" role="button" aria-pressed="true">Room Info</a>
                                         <button type="button" className="btn btn-danger" onClick={this.handleDeleteroom(room.roomId)}>Delete</button>
                                        
                                        </span>
                                        
                                }
                                </div>
                            </div>
                        )}
                   </div>
                  
                </div>
        
                }

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
