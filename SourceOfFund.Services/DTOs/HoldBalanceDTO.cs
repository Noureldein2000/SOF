using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.DTOs
{
    public class HoldBalanceDTO
    {
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public int? BalanceTypeId { get; set; }
        public int RequestId { get; set; }
        public int TransactionId { get; set; }
    }
}
