using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SourceOfFund.Data.Entities;

namespace SourceOfFund.Data.Configurations
{
    public class AccountServiceBalanceConfiguration : IEntityTypeConfiguration<AccountServiceBalance>
    {
        public void Configure(EntityTypeBuilder<AccountServiceBalance> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Balance).HasColumnType("decimal(18,3)");
            builder.HasOne(s => s.BalanceType).WithMany(s => s.AccountServiceBalances)
                .HasForeignKey(s => s.BalanceTypeID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
