import * as React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Button.css'
import './Container.css'
import * as Update from './Update'
import 'bootstrap/dist/css/bootstrap.min.css';
import { func } from 'prop-types';



function createPostCatalog(folder:any){
    return new Promise((succeed,fail)=>{
        
        var req = new XMLHttpRequest();
        req.open("POST", "/api/createCatalog");
        req.setRequestHeader('Content-type','application/json')
        
        //var json = JSON.parse(PathJson);
        req.addEventListener("load", function() {
        if (req.status < 400)
            succeed(req.response);
        else
            fail(new Error("Request failed: " + req.statusText));
        });
        req.addEventListener("error", function() {
            fail(new Error("Network error"));
        });
        req.send(folder);
    })
}

function succeesGetDataFunc(data:any)
{
    
}

function errorGetData(data:any){
    console.log(data);
}

function CreateCatalog_OnClick(e:any){
    var name = document.forms["formName"].elements["nameFileFolder"].value;

    var path = "/"+name;
    if (path!= null && 
        path.search("^\/(\w+)*(\/\w+)*$") == -1)
    {
        var desc = document.forms["formDescription"]
            .elements["DescriptionFileFolder"].value;
        var folder = JSON.stringify(
            {
                "Name":name,
                "Description":desc,
                "Extension":"",
                "Path":path
            }    
        );        
        var s = createPostCatalog(folder)
        .then(succeesGetDataFunc, errorGetData)
        .catch(errorGetData)
        var s = Update.getLookInsideCatalog(path)
        .then(Update.UpdateContainerFileFolders,Update.errorGetData)
        .catch(Update.errorGetData);
    }
    return false;
}

function UploadFile_OnClick(e:any){

}

function DeleteFilesFolders_OnClick(e:any){

}

function DownloadFile_OnClick(e:any){

}

export default class RightMenu extends React.Component{    
    render() {
        return (
            <div className="second">
                <div className="container">               
                <form id='formName'>
                    <div className="form-group">
                    <label htmlFor="nameFileFolder">Имя:</label>
                    <input type="text" className="form-control" id="nameFileFolder" />
                    </div>
                </form>   
                </div>              
                <div className="container">
                <form id='formDescription'>
                    <div className="form-group">
                    <label htmlFor="DescriptionFileFolder">Описание:</label>
                    <textarea className="form-control" rows={16} id="DescriptionFileFolder"></textarea>
                    </div>
                </form>
                </div>
                <div className="container">
                <div id="Buttons" className="second">
                    <div id="CreateNewFolder" className="one-button-container">
                        <button type="button" className="btn btn-block" onClick={CreateCatalog_OnClick}>
                            Создать каталог
                        </button>
                    </div>
                    <div id="UploadNewFiles" className="one-button-container">
                        <button className="btn btn-block" onClick={UploadFile_OnClick}>
                            Загрузить файл
                        </button>
                    </div>
                    <div id="DownLoadFile" className="one-button-container">
                        <button className="btn btn-block" onClick={DownloadFile_OnClick}>
                            Скачать файл
                        </button>
                    </div>
                    <div id="DeleteFilesFolders" className="one-button-container">
                        <button className="btn btn-block" onClick={DeleteFilesFolders_OnClick}>
                            Удалить
                        </button>
                    </div>
                </div>
                </div>
            </div>         
        );
    }
};