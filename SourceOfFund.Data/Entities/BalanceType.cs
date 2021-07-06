using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Entities
{
    public class BalanceType : BaseEntity<int>
    {
        public string Name { get; set; }
        public string ArName { get; set; }
        public virtual ICollection<AccountServiceBalance> AccountServiceBalances { get; set; }
        public virtual ICollection<AccountServiceAvailableBalance> AccountServiceAvailableBalances { get; set; }
        public virtual ICollection<BalanceHistory> BalanceHistorys { get; set; }    

    }
}
