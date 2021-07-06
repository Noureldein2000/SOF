using SourceOfFund.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Services
{
    public interface IAccountBalanceService
    {
        AccountBalanceDTO GetBalance(int accountId,int balanceTypeId,string languge);
    }
}
