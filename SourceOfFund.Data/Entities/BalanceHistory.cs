using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Entities
{
    public class BalanceHistory : BaseEntity<int>
    {
        public int TransactionID { get; set; }
        public decimal? BalanceBefore { get; set; }
        public int AccountID { get; set; }
        public int BalanceTypeID { get; set; }
        public virtual BalanceType BalanceType { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
