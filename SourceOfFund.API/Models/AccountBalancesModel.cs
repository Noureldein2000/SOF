using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API.Models
{
    public class AccountBalancesModel
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalAvailableBalance { get; set; }
        public string BalanceType { get; set; }
        public int BalanceTypeId { get; set; }
    }
}
