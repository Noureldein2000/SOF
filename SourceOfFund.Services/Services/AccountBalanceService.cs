using SourceOfFund.Data.Entities;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Infrastructure.Helpers;
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
        private readonly IBaseRepository<HoldBalance, int> _holdBalances;
        
        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IUnitOfWork unitOfWork
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
            _unitOfWork = unitOfWork;
            _holdBalances = holdBalances;
        }

        public void HoldAmount(decimal amount, int accountId, int balanceTypeId, int requestId)
        {
            var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            av.AccountID == accountId && av.BalanceTypeID == balanceTypeId).FirstOrDefault();
            if (availableBalance.Balance < amount)
                throw new SourceOfFundException("", "5");
            
            availableBalance.Balance -= amount;
            _holdBalances.Add(new HoldBalance
            {
                AccountID = accountId,
                Balance = amount,
                RequestID = requestId,
                SourceID = 1,
            });
            _unitOfWork.SaveChanges();
        }
        public AccountBalanceDTO GetBalance(int accountId, int balanceTypeId, string languge)
        {
          var accountBalanceDTO =  _accountServiceBalances.Getwhere(asb => asb.AccountID == accountId && asb.BalanceTypeID == balanceTypeId).Select(a=> new AccountBalanceDTO { TotalBalance = a.Balance }).FirstOrDefault();

           accountBalanceDTO = _accountServiceAvailableBalances.Getwhere(asb => asb.AccountID == accountId && asb.BalanceTypeID == balanceTypeId).Select(a=> new AccountBalanceDTO { TotalAvailableBalance = a.Balance }).FirstOrDefault();

            return accountBalanceDTO;
        }
    }
}
