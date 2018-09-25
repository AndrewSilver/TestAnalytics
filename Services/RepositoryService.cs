using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using smartAnalytics.Context;
using smartAnalytics.Interfaces;
using smartAnalytics.Models;

namespace smartAnalytics.Services
{
    public class RepositoryService : IRepositoryService
    {
        private readonly RepositoryContext _repositoryContext;
        private readonly IStorageService _storageService;
        public RepositoryService(RepositoryContext repositoryContext, IStorageService storageService)
        {
            _repositoryContext = repositoryContext;
            _storageService = storageService;
        }

        #region CreateMethods
        private async Task<FileModel> isExistFileAsync(string[] path, UserFile userFile)
        {
            int idFolder = await FindFolderAsync(path);
            var file = await _repositoryContext.Files.Where(a => a.FolderId == idFolder && 
                a.UserFile.Name == userFile.Name).FirstAsync();
            return (file); 
        }
        public async Task CreateFileAsync(UserFile userFile, string[] path, IFormFile formFile)
        {
            var tmp = await isExistFileAsync(path, userFile);
            if (tmp == null)
                return;
            var newFileModel = new FileModel()
            {
                UserFile = userFile,
                FolderId = tmp.Id,
                UniqName = await _storageService.CreateFileAsync(userFile.Name, formFile)
            };
            await _repositoryContext.Files.AddAsync(newFileModel);
            await _repositoryContext.SaveChangesAsync();
        }

        //проверка существования создаваемой папки в каталоге с путем path
        private async Task<(bool isExist, int folderId)> IsExistFolderAsync(string[] path, UserFile userFolder)
        {
            int idFolder = await FindFolderAsync(path);
            var folderWhereCreating = _repositoryContext.Storages.Where(fol => fol.InsideFolderId != null 
                && fol.FolderId == idFolder);
            var folderMaybeEqual = _repositoryContext.Folders.Where(a => a.UserFolder.Name == userFolder.Name);  
            var resultTmp = from fol in folderWhereCreating 
                            join fol2 in folderMaybeEqual on fol.InsideFolderId equals fol2.Id
                            select fol;
            return (resultTmp.Any() || idFolder==-1, idFolder);                          
        }

        //Создание папки в БД
        public async Task CreateFolderAsync(string[] path, UserFile userFolder)
        {
            var tmp = await IsExistFolderAsync(path, userFolder);
            if (tmp.isExist)
                return;
            FolderModel newFolder = new FolderModel()
            {
                UserFolder = userFolder
            };
            await _repositoryContext.Folders.AddAsync(newFolder);            
            //await _repositoryContext.SaveChangesAsync();
            var storage = await _repositoryContext.Storages.FirstOrDefaultAsync(p => p.FolderId == tmp.folderId 
                && p.InsideFolder == null);
            if (storage == null)
            {
                StorageModel newItemStorage = new StorageModel()
                {
                    FolderId = tmp.folderId,
                    InsideFolder = newFolder
                };
                await _repositoryContext.Storages.AddAsync(newItemStorage);            
            }
            else       
                storage.InsideFolder = newFolder;
            //await _repositoryContext.SaveChangesAsync();
            StorageModel tmpStorage = new StorageModel()
            {
                FolderId = newFolder.Id,
                InsideFolderId = null
            };
            await _repositoryContext.Storages.AddAsync(tmpStorage);            
            await _repositoryContext.SaveChangesAsync();
        }

        #endregion

        #region ReadMethods
        //Получить информацию о файл (имя, описание), 
        //который находится в папке idFile и имеет название=nameFile        
        public async Task<UserFile> GetFileInfoAsync(int idFile)
        {
            var mdl = await _repositoryContext.Files.FirstOrDefaultAsync(file => file.Id == idFile);
            return mdl.UserFile;
        }

        //Получить информацию о папке - имя, описание.
        public async Task<UserFile> GetFolderInfoAsync(int idFolder)
        {
            var mdl = await _repositoryContext.Folders.FirstOrDefaultAsync(fol => fol.Id == idFolder);
            return mdl.UserFolder;
        }

        //Получить файлы которые могут содержаться в папке с idFolder
        public async Task<ICollection<UserFile>> GetFilesAsync(int idFolder)
        {
            
            // var fld = await _repositoryContext.Folders.FirstOrDefaultAsync(f => f.Id == idFolder);
            // var files = fld.Files;
            // return files;
            //if (idFolder == -1)
            //    return null; 
            var tsk = _repositoryContext.Files.Where(a => a.FolderId == idFolder).ToListAsync();
            ICollection<UserFile> lstResult = new List<UserFile>();
            foreach (var t in await tsk)
                lstResult.Add(t.UserFile);
            return lstResult;
        }
                
        //Получить папки, которые содержаться в папке в idFolder
        public async Task<ICollection<UserFile>> GetFoldersAsync(int idFolder)
        {
            var tsk =  _repositoryContext.Storages.Where(a => a.FolderId == idFolder).ToListAsync();
            ICollection<UserFile> result = new List<UserFile>();
            foreach (var s in await tsk)
                if (s.InsideFolderId != null)
                    result.Add(s.InsideFolder.UserFolder);
            return result;
        }
        
        //Найти idFolder по path
        // path = /Folder/MyFolder
        public async Task<int> FindFolderAsync(string[] path)
        {
            //Лучше внедрять через IOption
            //path[0] = "Files";
            var tsk = _repositoryContext.Storages.Where(a => a.FolderId == 1).ToListAsync();
            //var tsk2 = await _repositoryContext.Storages.FirstOrDefaultAsync(a => a.FolderId == 1);
            int i = 0;
            int result = -1;
            bool isCatch = false;
            Queue<StorageModel> queueTmp = new Queue<StorageModel>();
            List<StorageModel> lstResultQuery = await tsk;
            int k = 0;
            while ((i < path.Length) && !isCatch)
            {
                foreach (var t in lstResultQuery)
                {
                    if ( (i+1) == path.Length && t.Folder.UserFolder.Name == path[i])
                    {
                        result = t.FolderId;
                        isCatch = true;
                        break;
                    }
                    if (t.InsideFolderId != null)
                    {
                        foreach(var tm in await _repositoryContext.Storages
                            .Where(b => b.FolderId == t.InsideFolderId).ToListAsync())
                            {                                
                                queueTmp.Enqueue(tm);
                            }
                    }                    
                    k++;
                }
                lstResultQuery.RemoveRange(0,k);
                k = 0;
                for (int q= 0; q < queueTmp.Count; q++)
                    lstResultQuery.Add(queueTmp.Dequeue());
                // for(int j = 0; (j < queueTmp.Count) && (!isCatch); j++)
                //     lstResultQuery.AddRange(await _repositoryContext.Storages
                //         .Where(b => b.FolderId == queueTmp.Dequeue().InsideFolderId).ToListAsync());
                i++;
            }
            return result;
        }
        
        public async Task<int> FindFileAsync(string[] path, string name)
        {
            int idFolder = -1;
            idFolder = await FindFolderAsync(path);
            var file = await _repositoryContext.Files.FirstOrDefaultAsync(f => f.UserFile.Name == name 
                && f.FolderId == idFolder);
            return file.Id;
        }

        #endregion

        #region DeleteMethods
        //Удалить файл из БД и файловой системы        
        public async Task DeleteFileAsync(int idFile)
        {
            var file = await _repositoryContext.Files.FirstOrDefaultAsync(f => f.Id == idFile);
            if (file != null){
                var uniqName = file.UniqName;
                _repositoryContext.Files.Remove(file);
                await _repositoryContext.SaveChangesAsync();
                StringBuilder path = new StringBuilder("wwwroot/Files/");//Не хорошо исправить
                path.Append(uniqName);
                _storageService.DeleteFile(path.ToString());                    
            }            
        }

        //Удалить папке с idFolder
        public async Task DeleteFolderAsync(int idFolder)
        {
            var folder = await _repositoryContext.Folders.FirstOrDefaultAsync(fol => fol.Id == idFolder);
            if (folder != null && folder.Id != 1)
            {
                var filesInFolder = await _repositoryContext.Files.Where(fi => fi.FolderId == folder.Id).ToListAsync();
                for (int i = 0; i < filesInFolder.Count; i++)
                {
                    await DeleteFileAsync(filesInFolder[i].Id);
                }
                _repositoryContext.Folders.Remove(folder);
                await _repositoryContext.SaveChangesAsync();
            }
        }
        
        #endregion

        
    }
}