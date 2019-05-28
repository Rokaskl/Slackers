import { combineReducers } from 'redux';

import { authentication } from './authentication.reducer';
import { registration } from './registration.reducer';
import { users } from './users.reducer';
import { rooms } from './rooms.reducer';
import { times } from './times.reducer';

import { alert } from './alert.reducer';

const rootReducer = combineReducers({
  authentication,
  registration,
  users,
  rooms,
  times,
  alert
});

export default rootReducer;