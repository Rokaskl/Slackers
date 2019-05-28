import { roomConstants } from '../_constants';

export function times(state = {}, action) {
  switch (action.type) {
    case roomConstants.GETTIMES_REQUEST:
      return {
        loading: true
      };
    case roomConstants.GETTIMES_SUCCESS:
      return {
        items: action.times
      };
    case roomConstants.GETTIMES_FAILURE:
      return { 
        error: action.error
      };
    
   
    default:
      return state
  }
}