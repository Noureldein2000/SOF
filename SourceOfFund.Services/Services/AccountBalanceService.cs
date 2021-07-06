using SourceOfFund.Data.Entities;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        private readonly IBaseRepository<AccountServiceAvailableBalance, int> _accountServiceAvailableBalances;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
        }

        public AccountBalanceDTO GetBalance(int accountId, int balanceTypeId, string languge)
        {
          var accountBalanceDTO =  _accountServiceBalances.Getwhere(asb => asb.AccountID == accountId && asb.BalanceTypeID == balanceTypeId).Select(a=> new AccountBalanceDTO { TotalBalance = a.Balance }).FirstOrDefault();

           accountBalanceDTO = _accountServiceAvailableBalances.Getwhere(asb => asb.AccountID == accountId && asb.BalanceTypeID == balanceTypeId).Select(a=> new AccountBalanceDTO { TotalAvailableBalance = a.Balance }).FirstOrDefault();

            return accountBalanceDTO;
        }
    }
}
