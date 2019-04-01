import React from 'react';
import { userActions } from '../_actions';

function RoomCard (props){ 
        return (
            <div>
                <div className="card">
                  <h4 className="card-title"> {props.room.roomName}</h4>
                  <p className="card-text">
                    Group id : {props.room.roomId}
                  
                    <br />
                  </p>
                </div>
                
                  
                
            </div>
          


            );
        }
export default RoomCard
    