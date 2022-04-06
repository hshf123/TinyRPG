using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
    class GameDbContext : DbContext
    {
        public DbSet<AccountDb> Account { get; set; }
        public DbSet<PlayerDb> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("");
        }
    }
}
