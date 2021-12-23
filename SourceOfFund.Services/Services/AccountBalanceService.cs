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
            var balanceTypeId = new SqlParameter("@BalanceTypeID", model.BalanceTypeId);
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
            var balanceTypeId = new SqlParameter("@BalanceTypeID", model.BalanceTypeId);
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
            var balanceTypeId = new SqlParameter("@BalanceTypeID", model.BalanceTypeId);
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

        public void ManageBalance(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId, bool save = true, int accountTypeId = 1)
        {
            //    var fromAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();
            //    var fromAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == toAccountId && x.BalanceTypeID == accountTypeId).FirstOrDefault();

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == toAccountId && x.BalanceTypeID == accountTypeId).FirstOrDefault();

            if (toAccountBalance == null || toAccountAvaliableBalance == null)
                throw new SourceOfFundException("", "5");

            //fromAccountAvaliableBalance.Balance -= amount;
            //fromAccountBalance.Balance -= amount;
            HoldAmount(new HoldBalanceDTO
            {
                AccountId = fromAccountId,
                Amount = amount,
                RequestId = accountFromRequestId,
                BalanceTypeId = accountTypeId
            });
            ConfirmAmount(new HoldBalanceDTO
            {
                AccountId = fromAccountId,
                Amount = amount,
                RequestId = accountFromRequestId,
                TransactionIds = new List<int> { accountFromTransactionId },
                BalanceTypeId = accountTypeId
            });
            toAccountAvaliableBalance.Balance += amount;
            toAccountBalance.Balance += amount;

            if (save)
                _unitOfWork.SaveChanges();
        }
        public void ManageBalanceForCashoutUniversity(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId)
        {
            //    var fromAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();
            //    var fromAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == fromAccountId).FirstOrDefault();

            var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == toAccountId && x.BalanceTypeID == 1).FirstOrDefault();

            var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == toAccountId && x.BalanceTypeID == 1).FirstOrDefault();

            if (toAccountBalance == null || toAccountAvaliableBalance == null)
                throw new SourceOfFundException("", "5");

            //fromAccountAvaliableBalance.Balance -= amount;
            //fromAccountBalance.Balance -= amount;
            HoldAmount(new HoldBalanceDTO
            {
                AccountId = fromAccountId,
                Amount = amount,
                RequestId = accountFromRequestId,
                BalanceTypeId = 3
            });
            ConfirmAmount(new HoldBalanceDTO
            {
                AccountId = fromAccountId,
                Amount = amount,
                RequestId = accountFromRequestId,
                TransactionIds = new List<int> { accountFromTransactionId },
                BalanceTypeId = 3
            });
            toAccountAvaliableBalance.Balance += amount;
            toAccountBalance.Balance += amount;

            _unitOfWork.SaveChanges();
        }
        public void CreateAccount(int accountId, decimal amount, List<int> balanceTypeIds)
        {
            var toAccountBalance = balanceTypeIds.Except(_accountServiceBalances.Getwhere(x => x.AccountID == accountId).Select(x => x.BalanceTypeID).ToList());
            //var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == accountId && balanceTypeIds.Contains(x.BalanceTypeID)).AsNoTracking().ToList();
            //var toAccountBalance = _accountServiceBalances.Getwhere(x => x.AccountID == accountId).AsNoTracking().ToList();

            var toAccountAvaliableBalance = balanceTypeIds.Except(_accountServiceAvailableBalances.Getwhere(x => x.AccountID == accountId).Select(x => x.BalanceTypeID).ToList());
            //var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == accountId && balanceTypeIds.Contains(x.BalanceTypeID)).AsNoTracking().ToList();
            //var toAccountAvaliableBalance = _accountServiceAvailableBalances.Getwhere(x => x.AccountID == accountId).AsNoTracking().ToList();

            //if (toAccountBalance.Count() == 0 || toAccountAvaliableBalance.Count == 0)
            //    throw new SourceOfFundException("", "5");

            if (toAccountBalance.Count() != toAccountAvaliableBalance.Count() || toAccountBalance.Count() == 0 || toAccountAvaliableBalance.Count() == 0)
                throw new SourceOfFundException("", "5");

            //if (toAccountBalance.Count() != toAccountAvaliableBalance.Count() || toAccountBalance.Count() == 0 || toAccountAvaliableBalance.Count() == 0)
            //    throw new SourceOfFundException("", "5");

            foreach (var balanceTypeId in toAccountBalance)
            {
                //if (!toAccountBalance.Any(x => x.BalanceTypeID == balanceTypeId))
                _accountServiceBalances.Add(new AccountServiceBalance
                {
                    AccountID = accountId,
                    Balance = amount,
                    BalanceTypeID = balanceTypeId
                });

                //if (!toAccountAvaliableBalance.Any(x => x.BalanceTypeID == balanceTypeId))
                _accountServiceAvailableBalances.Add(new AccountServiceAvailableBalance
                {
                    AccountID = accountId,
                    Balance = amount,
                    BalanceTypeID = balanceTypeId
                });
            }

            _unitOfWork.SaveChanges();
        }

        public void ChangeStatus(int accountId, int requestId)
        {
            var requestIdParam = new SqlParameter("@RequestID", requestId);
            var accountIdParam = new SqlParameter("@AccountID", accountId);
            _context.Database.ExecuteSqlRaw(" [dbo].[ChangeHoldBalancStatus] @RequestID, @AccountID", requestIdParam, accountIdParam);
        }

        public void AddCommission(List<AccountCommissionDTO> commissions)
        {
            commissions.ForEach(commission =>
            {
                ManageBalance(Constants.AccountCommission, commission.AccountId, commission.Amount, commission.RequestId, commission.TransactionId, save: false);
            });
            _unitOfWork.SaveChanges();
        }

        public List<BalanceTypeDTO> GetBalanceTypes(string language)
        {
            return _balanceType.GetAll().Select(x => new BalanceTypeDTO
            {
                Id = x.ID,
                Name = language == "en" ? x.Name : x.ArName
            }).ToList();
        }

        public void SeedBalances(int accountId, List<SeedBalancesDTO> model)
        {
            //var totalAmount = model.Sum(s => s.Amount);

            var accounts = model.Select(s => s.AccountId).ToList();
            var accountBalances = _accountServiceBalances.Getwhere(s => accounts.Contains(s.AccountID) && s.BalanceTypeID == 3).ToList();
            var accountsAvailableBalances = _accountServiceAvailableBalances.Getwhere(s => accounts.Contains(s.AccountID) && s.BalanceTypeID == 3).ToList();

            model.ForEach(data =>
            {
                var selectedAccount = accountBalances.Where(s => s.AccountID == data.AccountId).FirstOrDefault();
                var selectedAvailableAccount = accountsAvailableBalances.Where(s => s.AccountID == data.AccountId).FirstOrDefault();
                if (selectedAccount != null && selectedAvailableAccount != null)
                {
                    selectedAccount.Balance += data.Amount;
                    selectedAvailableAccount.Balance += data.Amount;

                    HoldAmount(new HoldBalanceDTO
                    {
                        AccountId = accountId,
                        Amount = data.Amount,
                        RequestId = data.RequestId,
                        BalanceTypeId = 1
                    });
                    ConfirmAmount(new HoldBalanceDTO
                    {
                        AccountId = accountId,
                        Amount = data.Amount,
                        RequestId = data.RequestId,
                        TransactionIds = new List<int> { data.TrasnsactionId },
                        BalanceTypeId = 1
                    });
                }
            });

            _unitOfWork.SaveChanges();
        }

        public List<int> GetBalanceTypesByAccountId(int id, string language)
        {
            return _accountServiceBalances.Getwhere(x => x.AccountID == id).Select(x => x.BalanceTypeID).ToList();
        }

        public bool CheckSeedBalances(List<SeedBalancesDTO> model)
        {
            var accounts = model.Select(s => s.AccountId).ToList();
            var checkbalance = _accountServiceBalances.Getwhere(s => s.BalanceTypeID == 3 & accounts.Contains(s.AccountID)).ToList();
            var checkAvaialbeBalance = _accountServiceBalances.Getwhere(s => s.BalanceTypeID == 3 & accounts.Contains(s.AccountID)).ToList();
            return checkAvaialbeBalance.Count == accounts.Count && checkbalance.Count == accounts.Count;
        }
    }
}
