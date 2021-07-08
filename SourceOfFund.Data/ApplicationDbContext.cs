using Microsoft.EntityFrameworkCore;
using SourceOfFund.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public virtual DbSet<HoldBalance> HoldBalances { get; set; }
        public virtual DbSet<BalanceHistory> BalanceHistories { get; set; }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity<int> && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Modified:
                        ((BaseEntity<int>)entityEntry.Entity).UpdateDate = DateTime.Now;
                        break;
                    case EntityState.Added:
                        ((BaseEntity<int>)entityEntry.Entity).CreationDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
                
            }

            return base.SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
