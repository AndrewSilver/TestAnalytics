using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace smartAnalytics.Interfaces
{
    public interface IStorageService
    {
        Task<string> CreateFileAsync(string name, IFormFile file);
        Task<MemoryStream> GetFileAsync(string name);
        void DeleteFile(string name);
    }
}