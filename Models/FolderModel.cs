using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAnalytics.Models
{
    public class FolderModel
    {
        public int Id {get;set;}
        public virtual UserFile UserFolder {get;set;}
        public virtual ICollection<FileModel> Files {get;set;}

        [InverseProperty("Folder")]
        public  virtual ICollection<StorageModel> Storages {get;set;}
        
        [InverseProperty("InsideFolder")]
        public  virtual ICollection<StorageModel> InsideStorages {get;set;}
    }
}