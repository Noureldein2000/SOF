using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.DTOs
{
    public class AccountBalancesDTO
    {
        public decimal TotalBalance { get; set; }
        public decimal TotalAvailableBalance { get; set; }
        public string BalanceType { get; set; }
        public int BalanceTypeId { get; set; }
    }
}
