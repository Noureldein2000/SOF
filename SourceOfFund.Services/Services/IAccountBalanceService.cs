using SourceOfFund.Infrastructure;
using SourceOfFund.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Services
{
    public interface IAccountBalanceService
    {
        void HoldAmount(HoldBalanceDTO model);
        void RefundAmount(HoldBalanceDTO model);
        void ConfirmAmount(HoldBalanceDTO model);
        void ReturnBalance(int fromAccountId, int toAccountId, decimal amount);
        void ConfirmTransfer(int fromAccountId, int toAccountId, int requestId);
        void ManageBalance(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId, bool save = true, int accountTypeId = 1);
        AccountBalanceDTO GetBalance(int accountId, int balanceTypeId);
        void CreateAccount(int accountId, decimal amount,List<int> balanceTypeIds);
        void ChangeStatus(int accountId, int requestId);
        void AddCommission(List<AccountCommissionDTO> commissions);
        List<BalanceTypeDTO> GetBalanceTypes(string language);
        void SeedBalances(int accountId, List<SeedBalancesDTO> model);
        void ManageBalanceForCashoutUniversity(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId);
        List<int> GetBalanceTypesByAccountId(int id,string language);
    }
}
