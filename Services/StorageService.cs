
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using smartAnalytics.Interfaces;

namespace smartAnalytics.Services
{
    public class StorageService : IStorageService
    {
        public readonly IFileProvider _fileProvider;

        public StorageService(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<string> CreateFileAsync(string name, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            StringBuilder uniqName = new StringBuilder(Guid.NewGuid().ToString());
            uniqName.Append(name);
            using (FileStream stream = new FileStream($"wwwroot/Files/{uniqName}", FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return uniqName.ToString();
        }

        public void DeleteFile(string name)
        {
            IFileInfo file = _fileProvider.GetFileInfo(name.ToString());
            if (file.Exists)
                System.IO.File.Delete(file.PhysicalPath);
        }

        public async Task<MemoryStream> GetFileAsync(string name)
        {
            MemoryStream memory = new MemoryStream();
            using (FileStream stream = new FileStream($"wwwroot/Files/{name}", FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return memory;
        }
    }
}