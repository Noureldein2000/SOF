﻿using SourceOfFund.Data.Entities;
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
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace SourceOfFund.Services.Services
{
    public class AccountBalanceService : IAccountBalanceService
    {
        private readonly IBaseRepository<AccountServiceBalance, int> _accountServiceBalances;
        private readonly IBaseRepository<AccountServiceAvailableBalance, int> _accountServiceAvailableBalances;
        private readonly IBaseRepository<HoldBalance, int> _holdBalances;
        private readonly IBaseRepository<BalanceType, int> _balanceType;
        private readonly IBaseRepository<BalanceHistory, int> _balanceHistory;
        private static ILogger<AccountBalanceService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IBaseRepository<BalanceType, int> balanceType,
            IBaseRepository<BalanceHistory, int> balanceHistory,
            IUnitOfWork unitOfWork,
            ILogger<AccountBalanceService> logger,
            ApplicationDbContext context
            )
        {
            _accountServiceBalances = accountServiceBalances;
            _accountServiceAvailableBalances = accountServiceAvailableBalances;
            _unitOfWork = unitOfWork;
            _holdBalances = holdBalances;
            _balanceType = balanceType;
            _balanceHistory = balanceHistory;
            _logger = logger;
            _context = context;
        }

        public void HoldAmount(HoldBalanceDTO model)
        {
            var sourceId = new SqlParameter("@SourceID", 1);
            var requestId = new SqlParameter("@RequestID", model.RequestId);
            var accountId = new SqlParameter("@AccountID", model.AccountId);
            var amount = new SqlParameter("@Amount", model.Amount);
            var balanceRequestTypeId = new SqlParameter("@BalanceRequestTypeID", 1);
            var balanceTypeId = new SqlParameter("@BalanceTypeID", 1);
            var transactionId = new SqlParameter("@TransactionID", 1);
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @TransactionID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, transactionId, statusCodeOutput);

            //_logger.LogInformation($"[Hold] request id {model.RequestId} value {statusCodeOutput.SqlValue}");
            

        }
        public void RefundAmount(HoldBalanceDTO model)
        {
            var sourceId = new SqlParameter("@SourceID", 1);
            var requestId = new SqlParameter("@RequestID", model.RequestId);
            var accountId = new SqlParameter("@AccountID", model.AccountId);
            var amount = new SqlParameter("@Amount", model.Amount);
            var balanceRequestTypeId = new SqlParameter("@BalanceRequestTypeID", 3);
            var balanceTypeId = new SqlParameter("@BalanceTypeID", 1);
            var transactionId = new SqlParameter("@TransactionID", 1);
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @TransactionID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, transactionId, statusCodeOutput);

            //_logger.LogInformation($"[Refund] request id {model.RequestId} value {statusCodeOutput.SqlValue}");
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
            var sourceId = new SqlParameter("@SourceID", 1);
            var requestId = new SqlParameter("@RequestID", model.RequestId);
            var accountId = new SqlParameter("@AccountID", model.AccountId);
            var amount = new SqlParameter("@Amount", model.Amount);
            var balanceRequestTypeId = new SqlParameter("@BalanceRequestTypeID", 2);
            var balanceTypeId = new SqlParameter("@BalanceTypeID", 1);
            var transactionId = new SqlParameter("@TransactionID", model.TransactionIds.FirstOrDefault());
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @TransactionID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, transactionId, statusCodeOutput);

            //_logger.LogInformation($"[Confirm] request id {model.RequestId} value {statusCodeOutput.SqlValue}");

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

        public void ManageBalance(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            var fromAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == toAccountId).FirstOrDefault();

            if (toAccountBalance == null || toAccountAvaliableBalance == null)
                throw new SourceOfFundException("", "5");

            fromAccountAvaliableBalance.Balance -= amount;
            fromAccountBalance.Balance -= amount;

            toAccountAvaliableBalance.Balance += amount;
            toAccountBalance.Balance += amount;

            //switch (transactionType)
            //{
            //    case TransactionType.Increment:


            //        break;
            //    case TransactionType.Decrement:
            //        toAccountAvaliableBalance.Balance -= amount;
            //        toAccountBalance.Balance -= amount;


            //        break;
            //    default:
            //        break;
            //}
            _unitOfWork.SaveChanges();
        }

        public void CreateAccount(int accountId, decimal amount)
        {
            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == accountId).FirstOrDefault();

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == accountId).FirstOrDefault();

            if (toAccountBalance != null || toAccountAvaliableBalance != null)
                throw new SourceOfFundException("", "5");

            _accountServiceAvailableBalances.Add(new AccountServiceAvailableBalance
            {
                AccountID = accountId,
                Balance = amount,
                BalanceTypeID = 1
            });

            _accountServiceBalances.Add(new AccountServiceBalance
            {
                AccountID = accountId,
                Balance = amount,
                BalanceTypeID = 1
            });

            _unitOfWork.SaveChanges();
        }
    }
}
