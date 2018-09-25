import * as React from 'react';
import * as ReactDom from 'react-dom';
import { element } from 'prop-types';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import Folders from './Folders';
import RightMenu from './RightMenu';
import './Container.css'

export default class App extends React.Component{
    render() {
        return (
            <div id="MainContainer" className="flex-container">
                <Folders />
                <RightMenu/>
            </div>         
        );
    }
};
