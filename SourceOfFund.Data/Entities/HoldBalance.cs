using SourceOfFund.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Data.Entities
{
    public class HoldBalance : BaseEntity<int>
    {
        public int AccountID { get; set; }
        public int RequestID { get; set; }
        public decimal Balance { get; set; }
        public int SourceID { get; set; }
        public ActiveStatus Status { get; set; }
    }
}
