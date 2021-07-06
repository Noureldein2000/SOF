using SourceOfFund.Data.Entities;
using SourceOfFund.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances
            )
        {
            _accountServiceBalances = accountServiceBalances;
        }

    }
}
