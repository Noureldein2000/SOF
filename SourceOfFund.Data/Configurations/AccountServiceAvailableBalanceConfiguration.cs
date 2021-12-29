using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SourceOfFund.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Configurations
{
    public class AccountServiceAvailableBalanceConfiguration : IEntityTypeConfiguration<AccountServiceAvailableBalance>
    {
        public void Configure(EntityTypeBuilder<AccountServiceAvailableBalance> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Balance).HasColumnType("decimal(18,3)");
            builder.HasOne(s => s.BalanceType).WithMany(s => s.AccountServiceAvailableBalances)
                .HasForeignKey(s => s.BalanceTypeID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(s => new { s.AccountID, s.BalanceTypeID }).IsUnique(true).HasName("IX_AccountServiceAvailableBalances_AccountID_BalanceTypeID");

            //builder.Property(s => s.RowVersion).IsRowVersion().ValueGeneratedOnUpdate();
        }
    }
}
