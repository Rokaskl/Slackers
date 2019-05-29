import React from 'react';
import { Router, Route, Switch } from 'react-router-dom';
import { connect } from 'react-redux';

import { history } from '../_helpers';
import { alertActions } from '../_actions';
import { PrivateRoute } from '../_components';
import { HomePage } from '../HomePage';
import { LoginPage } from '../LoginPage';
import { RegisterPage } from '../RegisterPage';
import { AboutPage } from '../AboutPage';
import { RoomsPage } from '../RoomsPage';
import { ProfilePage } from '../ProfilePage';
import { RoomPage } from '../SingleRoomPage';
import { NavigationBar} from '../_components/NavBar.js';
import { StickyFooter} from '../_components/Footer.jsx';
class App extends React.Component {
    constructor(props) {
        super(props);

        const { dispatch } = this.props;
        history.listen((location, action) => {
            // clear alert on location change
            dispatch(alertActions.clear());
        });
    }
    render() {
        const { alert, authentication } = this.props;
        return (
            <div>
            <NavigationBar/>
                    <div>
                        {alert.message &&
                            <div className={`alert ${alert.type}`}>{alert.message}</div>
                        }
                        <Router history={history}>
                            <Switch>
                        
                                <Route exact path="/" component={AboutPage} />
                                <Route path="/login" component={LoginPage} />
                                <Route path="/register" component={RegisterPage} />
                                <Route path="/about" component={AboutPage} />
                                <PrivateRoute exact path="/rooms" component={RoomsPage} />
                                <PrivateRoute path={`/:id`} component={RoomPage} />
                                <PrivateRoute exact path="/profilePage" component={ProfilePage} />
              
                            </Switch>
                        </Router>
                    </div>
              <StickyFooter/>
              </div>
        );
    }
}

  
function requireAuth(nextState, replace, next, authentication) {
    if(this.props.authentication.loggedIn == undefined || this.props.authentication.loggedIn ==false)
    {
      replace({
        pathname: "/login",
        state: {nextPathname: nextState.location.pathname}
      });
    }
    next();
  }

function mapStateToProps(state) {
    const { alert, authentication  } = state;

    return {
        alert,
        authentication
    };
}

const connectedApp = connect(mapStateToProps)(App);
export { connectedApp as App };
