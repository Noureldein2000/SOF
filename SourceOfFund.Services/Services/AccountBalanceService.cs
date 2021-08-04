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
        private readonly IServiceProvider _serviceProvider;
        private static ILogger<AccountBalanceService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public AccountBalanceService(
            IBaseRepository<AccountServiceBalance, int> accountServiceBalances,
            IBaseRepository<AccountServiceAvailableBalance, int> accountServiceAvailableBalances,
            IBaseRepository<HoldBalance, int> holdBalances,
            IBaseRepository<BalanceType, int> balanceType,
            IBaseRepository<BalanceHistory, int> balanceHistory,
            IServiceProvider serviceProvider,
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
            _serviceProvider = serviceProvider;
            _logger = logger;
            _context = context;
        }

        public void HoldAmount(HoldBalanceDTO model)
        {
            //var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
            //&& c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();
            //if (holdBalance != null)
            //    throw new SourceOfFundException("", "5");
            var sourceId = new SqlParameter("@SourceID", 1);
            var requestId = new SqlParameter("@RequestID", model.RequestId);
            var accountId = new SqlParameter("@AccountID", model.AccountId);
            var amount = new SqlParameter("@Amount", model.Amount);
            var balanceRequestTypeId = new SqlParameter("@BalanceRequestTypeID", 1);
            var balanceTypeId = new SqlParameter("@BalanceTypeID", 1);
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            var returnValue = _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, statusCodeOutput);

            _logger.LogInformation($"[Hold] request id {model.RequestId} value {statusCodeOutput.SqlValue}");
            //var availableBalance = _context.AccountServiceAvailableBalances.Where(av =>
            //        av.AccountID == model.AccountId && (model.BalanceTypeId.HasValue ? av.BalanceTypeID == model.BalanceTypeId : av.BalanceTypeID == 1)).FirstOrDefault();

            //if (availableBalance == null || availableBalance.Balance < model.Amount)
            //    throw new SourceOfFundException("", "5");

            //availableBalance.Balance -= model.Amount;
            //_context.HoldBalances.Add(new HoldBalance
            //{
            //    AccountID = model.AccountId,
            //    Balance = model.Amount,
            //    RequestID = model.RequestId,
            //    SourceID = 1,
            //    Status = ActiveStatus.True,
            //    AvailableBalanceBefore = availableBalance.Balance,
            //    BalanceTypeID = model.BalanceTypeId
            //});
            //try
            //{
            //    _context.SaveChanges();
            //    _logger.LogInformation($"[Hold] Succeeded");
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning($"I am in concurrent exception hold with request id {model.RequestId}");
            //    //foreach (var entry in ex.Entries)
            //    //{
            //    //    if (entry.Entity is AccountServiceAvailableBalance)
            //    //    {
            //    //        var proposedValues = entry.CurrentValues;
            //    //        var databaseValues = entry.GetDatabaseValues();
            //    //        _logger.LogWarning($"Before Value {proposedValues["Balance"]} for request id {model.RequestId}");
            //    //        //proposedValues["Balance"] = databaseValues["Balance"];
            //    //        entry.OriginalValues.SetValues(databaseValues);
            //    //        proposedValues["Balance"] = (decimal)proposedValues["Balance"] - model.Amount;
            //    //        _logger.LogWarning($"After Value {proposedValues["Balance"]} for request id {model.RequestId}");
            //    //    }
            //    //}
            //    HoldAmount(model);
            //}

        }
        public void RefundAmount(HoldBalanceDTO model)
        {
            var sourceId = new SqlParameter("@SourceID", 1);
            var requestId = new SqlParameter("@RequestID", model.RequestId);
            var accountId = new SqlParameter("@AccountID", model.AccountId);
            var amount = new SqlParameter("@Amount", model.Amount);
            var balanceRequestTypeId = new SqlParameter("@BalanceRequestTypeID", 3);
            var balanceTypeId = new SqlParameter("@BalanceTypeID", 1);
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            var returnValue = _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, statusCodeOutput);

            _logger.LogInformation($"[Refund] request id {model.RequestId} value {statusCodeOutput.SqlValue}");
            //var holdBalance = _holdBalances.Getwhere(c => c.RequestID == model.RequestId
            //&& c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();
            //if (holdBalance == null)
            //    throw new SourceOfFundException("", "5");

            //var availableBalance = _accountServiceAvailableBalances.Getwhere(av =>
            //av.AccountID == model.AccountId && av.BalanceTypeID == holdBalance.BalanceTypeID).FirstOrDefault();
            //if (availableBalance == null)
            //    throw new SourceOfFundException("", "5");

            //availableBalance.Balance += holdBalance.Balance;
            //holdBalance.Status = ActiveStatus.False;

            //_unitOfWork.SaveChanges();
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
            var statusCodeOutput = new SqlParameter("@StatusCode", 1);
            statusCodeOutput.Direction = ParameterDirection.Output;

            var returnValue = _context.Database.ExecuteSqlRaw(" [dbo].[ManageBalance] @SourceID, @RequestID, @AccountID, @Amount, @BalanceRequestTypeID, @BalanceTypeID, @StatusCode OUTPUT",
                sourceId, requestId, accountId, amount, balanceRequestTypeId, balanceTypeId, statusCodeOutput);

            _logger.LogInformation($"[Confirm] request id {model.RequestId} value {statusCodeOutput.SqlValue}");
            //var holdBalance = _context.HoldBalances.Where(c => c.RequestID == model.RequestId
            //        && c.AccountID == model.AccountId && c.Status == ActiveStatus.True).FirstOrDefault();

            //if (holdBalance == null)
            //    throw new SourceOfFundException(model.RequestId.ToString(), "5");

            //var balances = _context.AccountServiceAvailableBalances.Where(asb => asb.AccountID == model.AccountId).ToList();

            //if (balances.Count <= 0)
            //    throw new SourceOfFundException("", "5");
            //var totalBalances = balances.Sum(b => b.Balance);
            //var targetBalance = balances.Where(asb => asb.BalanceTypeID == holdBalance.BalanceTypeID).FirstOrDefault();

            //targetBalance.Balance -= holdBalance.Balance;
            //holdBalance.Status = ActiveStatus.False;
            //try
            //{

            //    _context.SaveChanges();
            //    _logger.LogInformation($"[Confirm] Succeeded");
            //    //Task.Run(() => 
            //    //CreateBalanceHistory(
            //    //    model.TransactionIds, model.AccountId, holdBalance.BalanceTypeID.Value,
            //    //    holdBalance.AvailableBalanceBefore, totalBalances);
            //    //);
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //    _logger.LogWarning($"I am in concurrent exception confirm request id {model.RequestId}");
            //    //foreach (var entry in ex.Entries)
            //    //{
            //    //    if (entry.Entity is AccountServiceBalance)
            //    //    {
            //    //        var proposedValues = entry.CurrentValues;
            //    //        var databaseValues = entry.GetDatabaseValues();
            //    //        _logger.LogWarning($"Before Value {proposedValues["Balance"]}");
            //    //        //proposedValues["Balance"] = databaseValues["Balance"];
            //    //        entry.OriginalValues.SetValues(databaseValues);
            //    //        proposedValues["Balance"] = (decimal)proposedValues["Balance"] - model.Amount;
            //    //        _logger.LogWarning($"Before Value {proposedValues["Balance"]}");
            //    //    }
            //    //}
            //    ConfirmAmount(model);
            //}


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
