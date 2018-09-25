
export function getLookInsideCatalog(PathJson:string){
    return new Promise((succeed,fail)=>{
        
        var req = new XMLHttpRequest();
        req.open("POST", "/api/browse");
        req.setRequestHeader('Content-type','application/json')
        var jso = JSON.stringify(
            {
                "Path":PathJson
            }    
        );
        req.addEventListener("load", function() {
        if (req.status < 400)
            succeed(req.response);
        else
            fail(new Error("Request failed: " + req.statusText));
        });
        req.addEventListener("error", function() {
            fail(new Error("Network error"));
        });
        req.send(jso);
    })
}
   
export function UpdateContainerFileFolders(data:any){
    var rootDiv = document.getElementById('FoldersContainer');
    while (rootDiv != null && rootDiv.firstChild) {
        rootDiv.removeChild(rootDiv.firstChild);
    }
    var arr = JSON.parse(data);
    for (var key in arr) {
        var a = document.createElement("a");
        a.textContent = arr[key].name;
        a.className ="child";
        var path:any;
        if (arr[key].path == "/")            
            path = arr[key].path + arr[key].name;            
        else 
            path = arr[key].path+"/"+arr[key].name;
        a.href = path;
        a.onclick = (e) => UpdateCatalog_OnClick(e);
                    
        if (rootDiv != null)
            rootDiv.appendChild(a);
        
    }
}

export function errorGetData(data:any){
    console.log(data);
}

export function UpdateCatalog_OnClick(e:any)
{   
    var cont = document.getElementById("ButtonBackContainer");
    var lbl = document.createElement("label");
    lbl.innerText = ">";
    
    var div = document.createElement("div");
    div.appendChild(lbl);
    var a = document.createElement("a");
    a.innerText = e.target.innerText;
    a.href = e.target.getAttribute("href");
    a.onclick = (e) => UpdateBackButton_OnClick(e);
    
    div.appendChild(a);
    if (cont != null)
    {
        cont.appendChild(div);
    }

    var s = getLookInsideCatalog(e.target.getAttribute("href"))
        .then(UpdateContainerFileFolders,errorGetData)
        .catch(errorGetData)
    return false;
}

export function UpdateContainerBackButtons(data:any){
    var rootDiv = document.getElementById('FoldersContainer');
    while (rootDiv != null && rootDiv.firstChild) {
        rootDiv.removeChild(rootDiv.firstChild);
    }
    var arr = JSON.parse(data);
    for (var key in arr) {
        var a = document.createElement("a");
        a.textContent = arr[key].name;
        a.className ="child";
        var path:any;
        if (arr[key].path == "/")            
            path = arr[key].path + arr[key].name;            
        else 
            path = arr[key].path+"/"+arr[key].name;
        a.href = path;
        a.onclick = (e) => UpdateCatalog_OnClick(e);                    
        if (rootDiv != null)
            rootDiv.appendChild(a);
    }
}

export function UpdateBackButton_OnClick(e:any)
{    
    var rootContainerBack = document.getElementById('ButtonBackContainer');
    if (rootContainerBack != null)
    {
        var children = rootContainerBack.childNodes;
        var i:number = 0;
        var isFind:boolean = false;
        for (; (i < children.length)&& !isFind; i++) 
        {            
            var element = children[i].childNodes;
            for (var k = i ; (k < element.length) && !isFind; ++k)
            {                
                isFind = element[k].localName == "a" && 
                    element[k].textContent == e.target.innerText;
            }
        }
        var n = children.length;
        for (var j = i; j< n; ++j){
            //children[j].removeEventListener("click");
            children[i].remove();
        }
    }
    var s = getLookInsideCatalog(e.target.getAttribute("href"))
        .then(UpdateContainerBackButtons,errorGetData)
        .catch(errorGetData)
    return false;
}