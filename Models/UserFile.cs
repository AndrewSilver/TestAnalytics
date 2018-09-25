using System;
using System.ComponentModel.DataAnnotations;

namespace smartAnalytics.Models
{
    public class UserFile
    {
        [StringLength(50, MinimumLength = 3)]
        public string Name {get;set;}
        [StringLength(300)]
        public string Description {get;set;}        
        public string Extension{get;set;}
        public string Path{get;set;}
        
    }
}