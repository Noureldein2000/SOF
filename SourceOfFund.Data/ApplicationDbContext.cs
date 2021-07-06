using Microsoft.EntityFrameworkCore;
using SourceOfFund.Data.Entities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SourceOfFund.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<BalanceType> BalanceTypes { get; set; }
        public virtual DbSet<AccountServiceAvailableBalance> AccountServiceAvailableBalances { get; set; }
        public virtual DbSet<AccountServiceBalance> AccountServiceBalances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
