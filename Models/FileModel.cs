
using System;
using System.ComponentModel.DataAnnotations;

namespace smartAnalytics.Models
{
    public class FileModel
    {
        public int Id {get;set;}        
        public string UniqName {get;set;}
        public virtual UserFile UserFile {get;set;}
        public int FolderId {get;set;} 
        public virtual FolderModel Folder {get;set;}

    }
}