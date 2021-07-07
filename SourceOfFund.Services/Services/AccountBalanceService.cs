using SourceOfFund.Data.Entities;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Infrastructure.Helpers;
using SourceOfFund.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceOfFund.Infrastructure;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        private readonly IBaseRepository<AccountServiceAvailableBalance, int> _accountServiceAvailableBalances;
        private readonly IBaseRepository<HoldBalance, int> _holdBalances;
        private readonly IBaseRepository<BalanceType, int> _balanceType;

        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IBaseRepository<BalanceType, int> balanceType,
            IUnitOfWork unitOfWork
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
            _unitOfWork = unitOfWork;
            _holdBalances = holdBalances;
            _balanceType = balanceType;
        }

        public void HoldAmount(HoldBalanceDTO model)
        {
            var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            av.AccountID == model.AccountId && av.BalanceTypeID == model.BalanceTypeId).FirstOrDefault();
            if (availableBalance == null || availableBalance.Balance < model.Amount)
                throw new SourceOfFundException("", "5");

            availableBalance.Balance -= model.Amount;
            _holdBalances.Add(new HoldBalance
            {
                AccountID = model.AccountId,
                Balance = model.Amount,
                RequestID = model.RequestId,
                SourceID = model.BalanceTypeId,
                Status=ActiveStatus.True
            });
            _unitOfWork.SaveChanges();
        }
        public void RefundAmount(HoldBalanceDTO model)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
            && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();
            if(holdBalance == null)
                throw new SourceOfFundException("", "5");

            var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            av.AccountID == model.AccountId && av.BalanceTypeID == holdBalance.SourceID).FirstOrDefault();
            if (availableBalance == null)
                throw new SourceOfFundException("", "5");

            availableBalance.Balance += holdBalance.Balance;
            holdBalance.Status = ActiveStatus.False;

            _unitOfWork.SaveChanges();
        }
        public AccountBalanceDTO GetBalance(int accountId, int balanceTypeId)
        {
            var balances = _balanceType.Getwhere(b => b.ID == balanceTypeId
            && b.AccountServiceBalances.Any(a => a.AccountID == accountId)
            && b.AccountServiceAvailableBalances.Any(a => a.AccountID == accountId))
                .Select(a => new
                {
                    Balance = a.AccountServiceBalances.Where(a => a.AccountID == accountId)
                        .Select(b => b.Balance).FirstOrDefault(),
                    AvailableBalance = a.AccountServiceAvailableBalances.Where(a => a.AccountID == accountId)
                        .Select(b => b.Balance).FirstOrDefault(),
                }).FirstOrDefault();
          //  var accountBalance = _accountServiceBalances.Getwhere(asb => asb.AccountID == accountId
          //       && asb.BalanceTypeID == balanceTypeId).FirstOrDefault();

          //var accountAvaliableBalance= _accountServiceAvailableBalances.Getwhere(asb => asb.AccountID == accountId && asb.BalanceTypeID == balanceTypeId).FirstOrDefault();
            
            return new AccountBalanceDTO
            {
                TotalBalance= balances.Balance,
                TotalAvailableBalance= balances.AvailableBalance
            };
        }

        public void ConfirmAmount(HoldBalanceDTO model)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
           && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();

            if (holdBalance == null)
                throw new SourceOfFundException("", "5");

            var balance = _accountServiceBalances.Getwhere(asb =>
            asb.AccountID == model.AccountId && asb.BalanceTypeID == holdBalance.SourceID).FirstOrDefault();

            if (balance == null)
                throw new SourceOfFundException("", "5");

            balance.Balance -= holdBalance.Balance;
            holdBalance.Status = ActiveStatus.False;

            _unitOfWork.SaveChanges();
        }
    }
}
