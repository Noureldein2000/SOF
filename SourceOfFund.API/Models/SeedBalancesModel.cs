using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API.Models
{
    public class SeedBalancesModel
    {
        [Required]
        public int AccountId { get; set; }
        public int TrasnsactionId { get; set; }
        public int RequestId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
