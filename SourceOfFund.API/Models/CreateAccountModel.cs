using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API.Models
{
    public class CreateAccountModel
    {
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public List<int> BalanceTypeIds { get; set; }
    }
}
