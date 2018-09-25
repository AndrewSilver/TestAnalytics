using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using smartAnalytics.Models;

namespace smartAnalytics.Interfaces
{
    public interface IRepositoryService
    {
        Task<ICollection<UserFile>> GetFoldersAsync(int idFolder);

        Task<int> FindFolderAsync(string[] path);

        Task<int> FindFileAsync(string[] path, string name);

        Task<UserFile> GetFileInfoAsync(int idFile);

        Task<UserFile> GetFolderInfoAsync(int idFolder);

        Task<ICollection<UserFile>> GetFilesAsync(int idFolder);

        Task DeleteFileAsync(int idFile);
        
        Task DeleteFolderAsync(int idFolder);

        Task CreateFileAsync(UserFile userFile, string[] path, IFormFile formFile);

        Task CreateFolderAsync(string[] path, UserFile userFolder);
    }
}