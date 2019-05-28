import { roomConstants } from '../_constants';
import { roomService } from '../_services';
import { alertActions } from './';

export const roomActions = {
    register,
    getAll,
    getById,
    delete: _delete,
    getTimes
};



function register(room) {
    return dispatch => {
        dispatch(request(room));

        roomService.register(room)
            .then(
                room => { 
                    dispatch(success());
                    dispatch(alertActions.success('Registration successful'));
                },
                error => {
                    dispatch(failure(error.toString()));
                    dispatch(alertActions.error(error.toString()));
                }
            );
    };

    function request(room) { return { type: roomConstants.REGISTER_REQUEST, room } }
    function success(room) { return { type: roomConstants.REGISTER_SUCCESS, room } }
    function failure(error) { return { type: roomConstants.REGISTER_FAILURE, error } }
}
function getTimes(from, to ,id) {
    return dispatch => {
        dispatch(request());

        roomService.getTimes(from,to,id)
            .then(
                times => dispatch(success(times)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: roomConstants.GETTIMES_REQUEST } }
    function success(times) { return { type: roomConstants.GETTIMES_SUCCESS, times } }
    function failure(error) { return { type: roomConstants.GETTIMES_FAILURE, error } }
}
function getAll() {
    return dispatch => {
        dispatch(request());

        roomService.getAll()
            .then(
                rooms => dispatch(success(rooms)),
                error => dispatch(failure(error.toString()))
            );
    };

    function request() { return { type: roomConstants.GETALL_REQUEST } }
    function success(rooms) { return { type: roomConstants.GETALL_SUCCESS, rooms } }
    function failure(error) { return { type: roomConstants.GETALL_FAILURE, error } }
}
function getById(id){
    return dispatch => {
        dispatch(request(id));

        roomService.getById(id)
            .then(
                room => dispatch(success(room)),
                error => dispatch(failure(id, error.toString()))
            );
    };
    function request(id) { return { type: roomConstants.GETBYID_REQUEST, id } }
    function success(room) { return { type: roomConstants.GETBYID_SUCCESS, room } }
    function failure(error) { return { type: roomConstants.GETBYID_FAILURE, error } }
}
// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    return dispatch => {
        dispatch(request(id));

        roomService.delete(id)
            .then(
                room => dispatch(success(id)),
                error => dispatch(failure(id, error.toString()))
            );
    };

    function request(id) { return { type: roomConstants.DELETE_REQUEST, id } }
    function success(id) { return { type: roomConstants.DELETE_SUCCESS, id } }
    function failure(id, error) { return { type: roomConstants.DELETE_FAILURE, id, error } }
}