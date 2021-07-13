using SourceOfFund.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Services
{
    public interface IAccountBalanceService
    {
        void HoldAmount(HoldBalanceDTO model);
        void RefundAmount(HoldBalanceDTO model);
        void ConfirmAmount(HoldBalanceDTO model);
        void ReturnBalance(int fromAccountId, int ToAccountId, decimal Amount, int? balanceTypeId);
        AccountBalanceDTO GetBalance(int accountId, int balanceTypeId);
    }
}
