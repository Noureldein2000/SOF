using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SourceOfFund.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Configurations
{
    public class BalanceHistoryConfiguration : IEntityTypeConfiguration<BalanceHistory>
    {
        public void Configure(EntityTypeBuilder<BalanceHistory> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.BalanceBefore).HasColumnType("decimal(18,3)");
            builder.HasOne(s => s.BalanceType).WithMany(s => s.BalanceHistorys)
                .HasForeignKey(s => s.BalanceTypeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
