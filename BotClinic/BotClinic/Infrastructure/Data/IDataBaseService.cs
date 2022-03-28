using BotClinic.Common.Models;
using BotClinic.Common.Models.Qualificatio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotClinic.Infrastructure.Data
{
   public interface IDataBaseService 
    {

         DbSet<UserModel> User { get; set; }
        DbSet<QualificationModel> Qualification { get; set; }
        Task<bool> SaveAsync();

       
    }
}
