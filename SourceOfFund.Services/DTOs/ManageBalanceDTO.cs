using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SourceOfFund.Services.DTOs
{
    public class ManageBalanceDTO
    {
        [Required]
        public int FromAccountId { get; set; }
        [Required]
        public int ToAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public int RequestId { get; set; }
        public int TransactionId { get; set; }
        public int BalanceTypeId { get; set; }
    }
}
