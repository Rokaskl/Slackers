import React from 'react';
import { userActions } from '../_actions';

function RoomCard (props){ 
        return (
           

                  
                
                  <div className="card-body">
                    <h4 className="card-title">{props.room.roomName}</h4>
                    <p className="card-text">This Group/Room created by:{props.room.roomAdminId} </p>
                  </div>
          
                
          


            );
        }
export default RoomCard
    