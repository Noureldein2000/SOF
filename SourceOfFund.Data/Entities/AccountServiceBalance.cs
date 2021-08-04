using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SourceOfFund.Data.Entities
{
    public class AccountServiceBalance : BaseEntity<int>
    {
        public int AccountID { get; set; }
        public decimal Balance { get; set; }
        public int BalanceTypeID { get; set; }
        public virtual BalanceType BalanceType { get; set; }
        //[Timestamp]
        //public byte[] RowVersion { get; set; }
    }
}
