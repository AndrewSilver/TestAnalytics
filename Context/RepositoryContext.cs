using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using smartAnalytics.Models;

namespace smartAnalytics.Context
{
    public class RepositoryContext:DbContext
    {
        public DbSet<FileModel> Files {get;set;}
        public DbSet<FolderModel> Folders {get;set;}
        public DbSet<StorageModel> Storages {get;set;}

        public RepositoryContext(DbContextOptions<RepositoryContext> options)
            :base(options)
        {
            //Database.EnsureCreated();//Заполнить обязательно этими данными,
            //т.к. это папка рут
            
         }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<StorageModel>()
            //     .HasOne(s => s.Folder)
            //     .WithOne(f => f.Storage)
            //     .HasForeignKey<StorageModel>(s => s.FolderId);
            // modelBuilder.Entity<StorageModel>()
            //     .HasOne(opa => opa.InsideFolder)
            //     .WithOne(nap => nap.InsideStorage)
            //     .HasForeignKey<StorageModel>(opa => opa.InsideFolderId);
            
            modelBuilder.Entity<FileModel>().OwnsOne(p => p.UserFile);
            modelBuilder.Entity<FolderModel>().OwnsOne(folder => folder.UserFolder);

            modelBuilder.Entity<FileModel>().ToTable("Files");
            modelBuilder.Entity<StorageModel>().ToTable("Storages");
            modelBuilder.Entity<FolderModel>().ToTable("Folders");
        }
    }
}