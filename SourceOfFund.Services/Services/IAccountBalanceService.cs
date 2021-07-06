using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Services
{
    public interface IAccountBalanceService
    {
        void HoldAmount(decimal amount, int accountId, int balanceTypeId, int requestId);
    }
}
