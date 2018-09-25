import * as React from 'react';
import * as ReactDom from 'react-dom';
import { element } from 'prop-types';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Container.css'
import * as Update from './Update'


function InitCatalog(path:string)
{
    var s = Update.getLookInsideCatalog(path)
        .then(Update.UpdateContainerFileFolders,Update.errorGetData)
        .catch(Update.errorGetData)
    return false;
}



export default class Folders extends React.Component{
    constructor(data:any){
        super(data);
        
        InitCatalog("/");
    }
    render() {
        return (
            <div  className="first" >
                <div id="ButtonBackContainer"  className="container">
                    <div>
                        <label>></label>
                        <a href="/" onClick={Update.UpdateBackButton_OnClick}>/</a>   
                    </div>             
                </div>
                <div id="FoldersContainer" className="first">

                </div>
            </div>         
        );
    }
};