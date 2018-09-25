using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace smartAnalytics.Models
{
    public class StorageModel
    {
        public int Id{get;set;}
           
        [ForeignKey("Folder")]
        public int FolderId {get;set;}//родитель       
        public virtual FolderModel Folder {get;set;}           
            
        [ForeignKey("InsideFolder")]
        public int? InsideFolderId {get;set;}//дочерний элемент
        public virtual FolderModel InsideFolder {get;set;}
    }
}