using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MomoViewer.Model;

namespace MomoViewer.Repository.DataAcess
{
    class DatabaseContext: DbContext
    {
        public DbSet<LinkInfo> Links { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./recent.db");
        }


    }
}
