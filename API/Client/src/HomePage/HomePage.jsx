import React from 'react';
import { Link } from 'react-router-dom';
import { connect } from 'react-redux';

import { userActions } from '../_actions';

import { roomActions } from '../_actions/room.actions.js';
import RoomCard from '../_components/RoomCard.jsx';
import {button,btn} from 'react-bootstrap';


class HomePage extends React.Component {
    componentDidMount() {
     this.props.dispatch(userActions.getAll());
     this.props.dispatch(roomActions.getAll());
    }

    handleDeleteUser(id) {
        return (e) => this.props.dispatch(userActions.delete(id));
        
    }
    handleDeleteRoom(id) {
        return (e) => this.props.dispatch(roomActions.delete(id));
    }
    handleGetUser(id)   {
        return (user) => this.props.dispatch(userActions.getById(id));
    }
    render() {
        const { user, users,rooms} = this.props;
       
       // const roomComponents = roomsData.map(rome => <RoomCard key={room.roomI} roomName={room.roomName} roomAdminId={room.roomAdminId}  />)
       
            return (
            <div className="col-md-6 col-md-offset-3">
                <h1>Hi {user.firstName}!</h1>
                <p>You're logged in with React!!</p>
                <h3>All registered users:</h3>
                {users.loading && <em>Loading users...</em>}
                {users.error && <span className="text-danger">ERROR: {users.error}</span>}
                {users.items &&
                    <ul>
                        {users.items.map((user, index) =>
                            <li key={user.id}>
                                
                                {user.firstName + ' ' + user.lastName}
                                {
                                    user.deleting ? <em> - Deleting...</em>
                                    : user.deleteError ? <span className="text-danger"> - ERROR: {user.deleteError}</span>
                                    : <span> - <a onClick={this.handleDeleteUser(user.id)}>Delete</a></span>
                                }
                            </li>
                        )}
                    </ul>
                }
                <div>
                 {rooms.loading && <em>Loading rooms...</em>}
                {rooms.error && <span className="text-danger">ERROR: {rooms.error}</span>}
                {rooms.items &&
                   <div className="card-deck">
                    
                        {rooms.items.map((room, index) =>
                            <div key={room.roomId}>
                            
                              
                                {RoomCard({room})}
                               
                                {
                                    room.deleting ? <em> - Deleting...</em>
                                    : room.deleteError ? <span className="text-danger"> - ERROR: {room.deleteError}</span>
                                    :   <span> 
                                         <button type="button" className="btn btn-danger" onClick={this.handleDeleteRoom(room.roomId)}>Delete</button>
                                        </span>
                                }
                                </div>
                             
                          
                            
                        )}
                    </div>
                 
        
                }
                </div>
                <p>
                    <Link to="/login">Logout</Link>
                </p>
            </div>
        );
    }
}

function mapStateToProps(state) {
    const { rooms,users, authentication } = state;
    const { user } = authentication;



    return {
        user,
        users,
        rooms
    };
}

const connectedHomePage = connect(mapStateToProps)(HomePage);
export { connectedHomePage as HomePage };