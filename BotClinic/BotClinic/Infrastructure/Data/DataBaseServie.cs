using BotClinic.Common.Models;
using BotClinic.Common.Models.Qualificatio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotClinic.Infrastructure.Data
{
    public class DataBaseServie : DbContext , IDataBaseService
    {
        public DataBaseServie(DbContextOptions options)
        :base(options)
        {
            Database.EnsureCreatedAsync();
            //Database.EnsureCreatedAsync();
        }



        public DataBaseServie()
        {
            Database.EnsureCreated();
            // Database.EnsureCreatedAsync();

        }


        public DbSet<UserModel> User { get; set; }

        public DbSet<QualificationModel> Qualification { get; set; }
        public async Task<bool> SaveAsync()
        {
            return (await SaveChangesAsync() > 0);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserModel>().ToContainer("User").HasPartitionKey("channel").HasNoDiscriminator().HasKey("id");
            modelBuilder.Entity<QualificationModel>().ToContainer("Qualification").HasPartitionKey("idUser").HasNoDiscriminator().HasKey("id");
           
        }
    }
}
