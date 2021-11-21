using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.DTOs
{
    public class AccountCommissionDTO
    {
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public int RequestId { get; set; }
        public decimal Amount { get; set; }
    }
}
