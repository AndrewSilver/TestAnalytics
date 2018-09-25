
using System;
using Microsoft.AspNetCore.Http;

namespace smartAnalytics.Models
{
    public class UploadFileModel
    {
        public UserFile UserFile {get;set;}
        
        public PathModel Path {get;set;}
        
        public IFormFile FormFile {get;set;}
    }
}