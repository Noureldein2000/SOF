using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API.Models
{
    public class HoldBalanceModel
    {
        [Required]
        public decimal Amount { get; set; }
    }
}
