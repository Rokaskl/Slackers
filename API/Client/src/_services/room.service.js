import config from 'config';
import { authHeader } from '../_helpers';

export const roomService = {
    register,
    getAll,
    getById,
    delete: _delete
};


function getAll() {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(`${config.apiUrl}/rooms`, requestOptions).then(handleResponse);
}

function getById(id) {
    const requestOptions = {
        method: 'GET',
        headers: authHeader()
    };

    return fetch(`${config.apiUrl}/rooms/${id}`, requestOptions).then(handleResponse);
}

function register(room) {
    const requestOptions = {
        method: 'POST',
        headers: authHeader(),
        body: JSON.stringify(room)

       
    };
    console.log(requestOptions.body)
    return fetch(`${config.apiUrl}/rooms/register`, requestOptions).then(handleResponse);
}



// prefixed function name with underscore because delete is a reserved word in javascript
function _delete(id) {
    const requestOptions = {
        method: 'DELETE',
        headers: authHeader()
    };

    return fetch(`${config.apiUrl}/rooms/${id}`, requestOptions).then(handleResponse);
}

function handleResponse(response) {
    return response.text().then(text => {
        const data = text && JSON.parse(text);
        if (!response.ok) {
            if (response.status === 401) {
               // location.reload(true);
            }

            const error = (data && data.message) || response.statusText;
            return Promise.reject(error);
        }

        return data;
    });
}
