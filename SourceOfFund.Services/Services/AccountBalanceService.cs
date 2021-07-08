using SourceOfFund.Data.Entities;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Infrastructure.Helpers;
using SourceOfFund.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SourceOfFund.Infrastructure;
using System.Threading.Tasks;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        private readonly IBaseRepository<AccountServiceAvailableBalance, int> _accountServiceAvailableBalances;
        private readonly IBaseRepository<HoldBalance, int> _holdBalances;
        private readonly IBaseRepository<BalanceType, int> _balanceType;
        private readonly IBaseRepository<BalanceHistory, int> _balanceHistory;

        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IBaseRepository<BalanceType, int> balanceType,
            IBaseRepository<BalanceHistory, int> balanceHistory,
            IUnitOfWork unitOfWork
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
            _unitOfWork = unitOfWork;
            _holdBalances = holdBalances;
            _balanceType = balanceType;
            _balanceHistory = balanceHistory;
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
                SourceID = 1,
                Status = ActiveStatus.True,
                AvailableBalanceBefore = availableBalance.Balance,
                BalanceTypeID = model.BalanceTypeId
            });
            _unitOfWork.SaveChanges();
        }
        public void RefundAmount(HoldBalanceDTO model)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
            && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();
            if (holdBalance == null)
                throw new SourceOfFundException("", "5");

            var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            av.AccountID == model.AccountId && av.BalanceTypeID == holdBalance.BalanceTypeID).FirstOrDefault();
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

            return new AccountBalanceDTO
            {
                TotalBalance = balances.Balance,
                TotalAvailableBalance = balances.AvailableBalance
            };
        }
        public void ConfirmAmount(HoldBalanceDTO model)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
           && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();

            if (holdBalance == null)
                throw new SourceOfFundException("", "5");

            var balances = _accountServiceBalances.Getwhere(asb => asb.AccountID == model.AccountId).ToList();

            if (balances.Count <= 0)
                throw new SourceOfFundException("", "5");
            var totalBalances = balances.Sum(b => b.Balance);
            var targetBalance = balances.Where(asb => asb.BalanceTypeID == holdBalance.BalanceTypeID).FirstOrDefault();

            targetBalance.Balance -= holdBalance.Balance;
            holdBalance.Status = ActiveStatus.False;
            _unitOfWork.SaveChanges();

            Task.Run(() => CreateBalanceHistory(
                model.TransactionId, model.AccountId, model.BalanceTypeId, 
                holdBalance.AvailableBalanceBefore, totalBalances));
            
        }
        public void CreateBalanceHistory(int transactionId, int accountId, int balanceTypeId, decimal beforeBalance, decimal totalBalance)
        {
            _balanceHistory.Add(new BalanceHistory
            {
                AccountID = accountId,
                BalanceBefore = beforeBalance,
                BalanceTypeID = balanceTypeId,
                TransactionID = transactionId,
                TotalBalance = totalBalance
            });
            _unitOfWork.SaveChanges();
        }
    }
}
