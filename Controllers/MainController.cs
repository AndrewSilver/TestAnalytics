using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using smartAnalytics.Interfaces;
using smartAnalytics.Models;

namespace smartAnalytics.Controllers
{
    [ApiController]
    [Route("/api")]
    public class MainController:ControllerBase
    {
        private readonly IFileProvider _fileProvider;
        private readonly IRepositoryService _repositoryService;
        private readonly string wwwrootPath = "wwwroot/Files/";
        public MainController(IFileProvider fileProvider, IRepositoryService repositoryService)
        {
            _fileProvider = fileProvider;
            _repositoryService = repositoryService;
        }

        //Входные параметры: 1) Путь до объекта, информацию о котором хотим получить
        [HttpPost]
        [Route("getInfo")]
        public async Task<ActionResult<UserFile>> GetInfo([FromBody]PathModel path)
        {
            UserFile infoModel = new UserFile();
            string[] stringRootFolder;
            if (isFile(path.Path))
            {    
                stringRootFolder = path.Path.Split('/');
                stringRootFolder[0] = "Files";
                string nameFile = stringRootFolder[stringRootFolder.Length-1];
                Array.Resize(ref stringRootFolder, stringRootFolder.Length - 1);
                int idFile = await _repositoryService.FindFileAsync(stringRootFolder,nameFile);
                infoModel = await _repositoryService.GetFileInfoAsync(idFile);
                // infoModel.Name = file.Name;
                // infoModel.Description = file.Description;
                // infoModel.Path = file.Path;
            }
            else if(isFolder(path.Path))
            {
                GetTrueFolderPath(out stringRootFolder, path.Path);
                var idFolder = await _repositoryService.FindFolderAsync(stringRootFolder);
                if (idFolder == 1)
                    return Ok();
                infoModel = await _repositoryService.GetFolderInfoAsync(idFolder);
                // infoModel.Description = folder.Description;
                // infoModel.Name = folder.Name;
            }
            return Ok(new UserFile(){
                Name = infoModel.Name, 
                Description = infoModel.Description,
                Extension = infoModel.Extension,
                Path = infoModel.Path
            });
        }

        //Входные параметры: 1)путь каталога
        [HttpPost]
        [Route("browse")]
        public async Task<IActionResult> ShowCatalog(PathModel path)
        {
            if (!ModelState.IsValid)
                return BadRequest("Неправильный путь");            
            if (!isFolder(path.Path))
                return BadRequest("Неправильный путь");
            ICollection<UserFile> collectionFiles = new List<UserFile>();
            string[] truePathFolder;
            GetTrueFolderPath(out truePathFolder, path.Path);
            int idFolder = await _repositoryService.FindFolderAsync(truePathFolder);
            var folders = await _repositoryService.GetFoldersAsync(idFolder);
            var files = await _repositoryService.GetFilesAsync(idFolder);     
            foreach (var folder in folders)
                collectionFiles.Add(new UserFile()
                {
                    Name = folder.Name,
                    Extension = folder.Extension,
                    Description = folder.Description,
                    Path = folder.Path
                });
            foreach (var file in files)
                collectionFiles.Add(new UserFile()
                {
                    Name = file.Name,
                    Extension = file.Extension,
                    Description = file.Description,
                    Path = file.Path
                });

            return Ok(collectionFiles);
        }

        //Входные параметры: 1) Путь, куда загружается.
        //                   2) Его описание
                          // 3) Файл     
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> UploadFile(UploadFileModel uploadFileModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("Невалидная модель");
            if (!isFile(uploadFileModel.Path.Path))
                return BadRequest("Ошибка в названии файла");
            string[] truePath;
            GetTrueFolderPath(out truePath, uploadFileModel.Path.Path);
            await _repositoryService.CreateFileAsync(uploadFileModel.UserFile, truePath ,uploadFileModel.FormFile);
            return Ok();
        }
        
        //Входной параметр: 1) путь, где мы создаем.
        //                  2) Модель каталога, содержащая имя и описание
        
        [Route("createCatalog")]
        [HttpPost]
        public async Task<IActionResult> CreateCatalog([FromBody]UserFile pathCatalog)
        {
            if (!ModelState.IsValid)
                return BadRequest("Невалидная модель");
            if (!isFolder(pathCatalog.Path))
                return BadRequest("Неправильный путь");
            string[] truePath;
            GetTrueFolderPath(out truePath, pathCatalog.Path);
            await _repositoryService.CreateFolderAsync(truePath, pathCatalog);
            return Ok();
        }

        //входные параметры: 1) Путь папки, где мы удаляем.
        //                   2) Пути удаляемых объектов   
        [HttpPost]
        [Route("delete")]
        public async Task<IActionResult> Delete([FromBody]DeleteAllModel paths)
        {
            if (!ModelState.IsValid)
                return BadRequest("Невалидная модель");
            foreach (var path in paths.PathCollection)
            {
                string[] stringRootFolder;
                if (isFile(path.Path))
                {    
                    stringRootFolder = path.Path.Split('/');
                    stringRootFolder[0] = "Files";
                    string nameFile = stringRootFolder[stringRootFolder.Length-1];
                    Array.Resize(ref stringRootFolder, stringRootFolder.Length - 1);
                    var idFolder = await _repositoryService.FindFolderAsync(stringRootFolder);
                    int idFile = await _repositoryService.FindFileAsync(stringRootFolder,nameFile);
                    await _repositoryService.DeleteFileAsync(idFolder);
                }
                else if(isFolder(path.Path))
                {
                    GetTrueFolderPath(out stringRootFolder, path.Path);
                    var idFolder = await _repositoryService.FindFolderAsync(stringRootFolder);
                    await _repositoryService.DeleteFolderAsync(idFolder);
                }
            }
            return Ok();
        }

        private void GetTrueFolderPath(out string[] truePath, string path)
        {
            if (!(path == "/"))
            {
                truePath = path.Split('/');
                truePath[0] = "Files";
                return;
            }
            truePath = new string[]{"Files"};
        }

        private bool isFile(string path)
        {
            return Regex.IsMatch(path, @"^\/([a-zA-Z]\/?)*([^\/\\:*?"+@"<>|:]?[a-zA-Zа-яА-Я0-9_.]+(\.doc|\.xls))$");
        }

        private bool isFolder(string path)
        {
            return Regex.IsMatch(path, @"^\/(\w+)*(\/\w+)*$");
        }
    }
}