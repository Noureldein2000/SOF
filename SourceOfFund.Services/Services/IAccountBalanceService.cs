﻿using SourceOfFund.Infrastructure;
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
        void ManageBalance(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId, bool save = true);
        AccountBalanceDTO GetBalance(int accountId, int balanceTypeId);
        void CreateAccount(int accountId, decimal amount);
        void ChangeStatus(int accountId, int requestId);
        void AddCommission(List<AccountCommissionDTO> commissions);
    }
}
