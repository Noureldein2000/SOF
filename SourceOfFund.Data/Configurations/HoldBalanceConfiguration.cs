using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SourceOfFund.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Configurations
{
    public class HoldBalanceConfiguration : IEntityTypeConfiguration<HoldBalance>
    {
        public void Configure(EntityTypeBuilder<HoldBalance> builder)
        {
            builder.HasKey(s => s.ID);
            builder.Property(s => s.Balance).HasColumnType("decimal(18, 3)");
        }
    }
}
