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
using SourceOfFund.Data;
using Microsoft.Extensions.DependencyInjection;
using SourceOfFund.API.Models;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        private readonly IBaseRepository<AccountServiceAvailableBalance, int> _accountServiceAvailableBalances;
        private readonly IBaseRepository<HoldBalance, int> _holdBalances;
        private readonly IBaseRepository<BalanceType, int> _balanceType;
        private readonly IBaseRepository<BalanceHistory, int> _balanceHistory;
        private readonly IServiceProvider _serviceProvider;

        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IBaseRepository<BalanceType, int> balanceType,
            IBaseRepository<BalanceHistory, int> balanceHistory,
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
            _unitOfWork = unitOfWork;
            _holdBalances = holdBalances;
            _balanceType = balanceType;
            _balanceHistory = balanceHistory;
            _serviceProvider = serviceProvider;
        }

        public void HoldAmount(HoldBalanceDTO model)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
            && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();
            if (holdBalance != null)
                throw new SourceOfFundException("", "5");

            var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            av.AccountID == model.AccountId && (model.BalanceTypeId.HasValue ? av.BalanceTypeID == model.BalanceTypeId : av.BalanceTypeID == 1)).FirstOrDefault();

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

            if (balances == null)
                throw new SourceOfFundException("", "0");

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
            //_unitOfWork.SaveChanges();

            //Task.Run(() => 
            CreateBalanceHistory(
                model.TransactionIds, model.AccountId, holdBalance.BalanceTypeID.Value,
                holdBalance.AvailableBalanceBefore, totalBalances);
            //);

        }
        public void CreateBalanceHistory(List<int> transactionIds, int accountId, int balanceTypeId, decimal beforeBalance, decimal totalBalance)
        {
            //using var scope = _serviceProvider.CreateScope();
            //var services = scope.ServiceProvider;
            //var context = services.GetRequiredService<ApplicationDbContext>();
            foreach (var transactionId in transactionIds)
            {
                _balanceHistory.Add(new BalanceHistory
                {
                    AccountID = accountId,
                    BalanceBefore = beforeBalance,
                    BalanceTypeID = balanceTypeId,
                    TransactionID = transactionId,
                    TotalBalance = totalBalance
                });
            }
            _unitOfWork.SaveChanges();
        }
        public void ReturnBalance(int fromAccountId, int toAccountId, decimal Amount)
        {
            var fromAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();
            fromAccountBalance.Balance -= Amount;

            var fromAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();
            fromAccountAvaliableBalance.Balance -= Amount;

            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();
            toAccountBalance.Balance += Amount;

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();
            toAccountAvaliableBalance.Balance += Amount;

            _unitOfWork.SaveChanges();

        }
        public void ConfirmTransfer(int fromAccountId, int toAccountId, int requestId)
        {
            var holdBalance = _holdBalances.Getwhere(c => c.RequestID == requestId
           && c.AccountID == fromAccountId && c.Status == ActiveStatus.True).FirstOrDefault();

            if (holdBalance == null)
                throw new SourceOfFundException("", "5");

            var balancesAccountFrom = _accountServiceBalances.Getwhere(asb => asb.AccountID == fromAccountId).ToList();

            var balancesAccountTo = _accountServiceBalances.Getwhere(asb => asb.AccountID == toAccountId).ToList();
            var avaliableBalancesAccountTo = _accountServiceAvailableBalances.Getwhere(asb => asb.AccountID == toAccountId).ToList();

            if (balancesAccountFrom.Count <= 0)
                throw new SourceOfFundException("", "5");

            //var totalBalancesFromAccount = balancesAccountFrom.Sum(b => b.Balance);
            var sourceBalance = balancesAccountFrom.Where(asb => asb.BalanceTypeID == 1).FirstOrDefault();
            var targetBalance = balancesAccountTo.Where(asb => asb.BalanceTypeID == 1).FirstOrDefault();
            var targetAvaliableBalance = avaliableBalancesAccountTo.Where(asb => asb.BalanceTypeID == 1).FirstOrDefault();

            sourceBalance.Balance -= holdBalance.Balance;
            targetBalance.Balance += holdBalance.Balance;
            targetAvaliableBalance.Balance += holdBalance.Balance;
            holdBalance.Status = ActiveStatus.False;

            _unitOfWork.SaveChanges();

        }

        public void ManageBalance(int fromAccountId, int toAccountId, decimal amount, TransactionType transactionType)
        {
            //var fromAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            //var fromAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();

            if(toAccountBalance == null || toAccountAvaliableBalance == null)
                throw new SourceOfFundException("", "5");

            switch (transactionType)
            {
                case TransactionType.Increment:
                    //fromAccountAvaliableBalance.Balance -= amount;
                    //fromAccountBalance.Balance -= amount;

                    toAccountAvaliableBalance.Balance += amount;
                    toAccountBalance.Balance += amount;

                    break;
                case TransactionType.Decrement:
                    toAccountAvaliableBalance.Balance -= amount;
                    toAccountBalance.Balance -= amount;

                    break;
                default:
                    break;
            }
            _unitOfWork.SaveChanges();
        }
    }
}
