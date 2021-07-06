using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.DTOs
{
   public class AccountBalanceDTO
    {

        public decimal TotalBalance { get; set; }
        public decimal TotalAvailableBalance { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
