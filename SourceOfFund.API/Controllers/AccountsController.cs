using Microsoft.AspNetCore.Mvc;
using SourceOfFund.API.Models;
using SourceOfFund.Infrastructure.Helpers;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Services.Models;
using SourceOfFund.Services.Services;
using System;
using System.Collections.Generic;
using Swashbuckle.AspNetCore;
using SourceOfFund.Infrastructure;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Data;

namespace SourceOfFund.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsController : BaseController
    {
        private readonly IAccountBalanceService _accountBalanceService;
        private static ILogger<AccountsController> _logger;
        public AccountsController(IAccountBalanceService accountBalanceService, ILogger<AccountsController> logger)
        {
            _accountBalanceService = accountBalanceService;
            _logger = logger;
        }

        [HttpGet]
        [Route("{accountId}/balances/{balanceTypeId}")]
        [ProducesResponseType(typeof(AccountBalanceResponseModel), 200)]
        public IActionResult GetBalance(int accountId, int balanceTypeId)
        {
            try
            {
                var balancesModel = _accountBalanceService.GetBalance(accountId, balanceTypeId);
                return Ok(new AccountBalanceResponseModel
                {
                    Code = 200,
                    TotalBalance = balancesModel.TotalBalance,
                    TotalAvailableBalance = balancesModel.TotalAvailableBalance
                });

            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, $"[GetBalance] {ex.Message}, account {accountId}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[GetBalance] {ex.Message}, account {accountId}");
                return Ok(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("{accountId}/balances/{balanceTypeId}/requests/{requestId}")]
        public IActionResult Post([FromBody] HoldBalanceModel model, int accountId, int requestId, int? balanceTypeId = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                _logger.LogInformation($"[Hold] request id: {requestId}, account id: {accountId}, amount: {model.Amount}");
                _accountBalanceService.HoldAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    Amount = model.Amount,
                    RequestId = requestId,
                    BalanceTypeId = balanceTypeId.HasValue ? balanceTypeId.Value : 1
                });
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, $"[Post SOF Exception] {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
                return Ok(dbex.Message, "0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Post Exception] {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }
        [HttpDelete]
        [Route("{accountId}/requests/{requestId}")]
        public IActionResult Refund([FromRoute] int accountId, int requestId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                _logger.LogInformation($"[Refund] request id: {requestId}, account id: {accountId}");
                _accountBalanceService.RefundAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                });

            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, "[Refund Exception]");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Refund Exception]");
                return Ok(ex.Message, "0");
            }
            return Ok("Success", "200");
        }

        [HttpPut]
        [Route("{accountId}/requests/{requestId}")]
        public IActionResult Confirm(int accountId, int requestId, [FromQuery] int[] transactionIds)
        {
            try
            {
                if (!ModelState.IsValid)
                    return Ok(ModelState);

                _logger.LogInformation($"[Confirmed] request id: {requestId}, account id: {accountId}");
                _accountBalanceService.ConfirmAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                    TransactionIds = transactionIds.ToList()
                });

                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[Confirmed Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
                return Ok("Success", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Confirm Exception] {ex.Message}");
            }
            return Ok("Success", "200");
        }

        [HttpPut]
        [Route("ReturnBalance/{fromAccountId}/{toAccountId}/balances/{amount}")]
        public IActionResult ReturnBalance(int fromAccountId, int toAccountId, decimal amount)
        {
            try
            {
                _logger.LogInformation($"[ReturnBalance] account from id: {fromAccountId}, account to id: {toAccountId} amount: {amount}");
                _accountBalanceService.ReturnBalance(fromAccountId, toAccountId, amount);
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[ReturnBalance Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ReturnBalance Error1] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }

        [HttpPost]
        [Route("ConfirmTransfer/{fromAccountId}/{toAccountId}/requests/{requestId}")]
        public IActionResult ConfirmTransfer(int fromAccountId, int toAccountId, int requestId)
        {
            try
            {
                _logger.LogInformation($"[ConfirmTransfer] account from id: {fromAccountId}, account to id: {toAccountId} request id: {requestId}");
                _accountBalanceService.ConfirmTransfer(fromAccountId, toAccountId, requestId);
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[ConfirmTransfer Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ConfirmTransfer Error2] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }

        [HttpPost]
        [Route("ManageBalance/{fromAccountId}/{toAccountId}/balances/{amount}/requests/{accountFromRequestId}/transactions/{accountFromTransactionId}")]
        public IActionResult ManageBalance(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId)
        {
            try
            {
                _logger.LogInformation($"[ManageBalance] account from id: {fromAccountId}, account to id: {toAccountId} amount: {amount}, requestId: {accountFromRequestId}");
                _accountBalanceService.ManageBalance(fromAccountId, toAccountId, amount, accountFromRequestId, accountFromTransactionId);
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[ManageBalance Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ManageBalance Error1] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("ManageBalance")]
        public IActionResult ManageBalance([FromBody] ManageBalanceDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok("Not Valid", "-7");
                }
                _logger.LogInformation($"[ManageBalance] account from id: {model.FromAccountId}, account to id: {model.ToAccountId} amount: {model.Amount}, requestId: {model.RequestId}");
                _accountBalanceService.ManageBalanceForCashoutUniversity(model.FromAccountId, model.ToAccountId, model.Amount, model.RequestId, model.TransactionId);
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[ManageBalance2 Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ManageBalance2 Error1] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("CreateAccount")]
        public IActionResult CreateAccount([FromBody] CreateAccountModel model)
        {
            try
            {
                _logger.LogInformation($"[CreateAccount] account id: {model.AccountId}, amount: {model.Amount}, balanceType: {string.Join(", ", model.BalanceTypeIds)}");
                _accountBalanceService.CreateAccount(model.AccountId, model.Amount, model.BalanceTypeIds);
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[CreateAccount Error1] Message: {ex.Message}, TraceCode: {ex.StackTrace}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CreateAccount Error1] Message: {ex.Message}, TraceCode: {ex.StackTrace}");
                return Ok(ex.Message, "0");
            }
        }

        [HttpDelete]
        [Route("{accountId}/requests/{requestId}/changestatus")]
        public IActionResult ChangeStatus([FromRoute] int accountId, int requestId)
        {
            try
            {
                _accountBalanceService.ChangeStatus(accountId, requestId);
                _logger.LogInformation($"[Change Status] request id: {requestId}, account id: {accountId}");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, "[Change Status Exception]");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Change Status Exception]");
                return Ok(ex.Message, "0");
            }
            return Ok("Success", "200");
        }
        [HttpGet]
        [Route("BalanceTypes")]
        [ProducesResponseType(typeof(List<BalanceTypeModel>), 200)]
        public IActionResult GetBalanceTypes(string language = "ar")
        {
            try
            {
                var balancesTypes = _accountBalanceService.GetBalanceTypes(language).Select(x => new BalanceTypeModel
                {
                    Id = x.Id,
                    Name = x.Name
                });

                return Ok(balancesTypes);

            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("checkbalances/seed")]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult CheckSeedBalances([FromBody] List<SeedBalancesModel> model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok("Not Valid", "-7");
                }
                var result = _accountBalanceService.CheckSeedBalances(model.Select(s => new SeedBalancesDTO
                {
                    AccountId = s.AccountId
                }).ToList());

                return Ok(result);
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[CheckSeedBalances Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[CheckSeedBalances Error1] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }

        [HttpPost]
        [Route("{accountId}/balances/seed")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult SeedBalances(int accountId, [FromBody] List<SeedBalancesModel> model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok("Not Valid", "-7");
                }
                _logger.LogInformation($"[SeedBalances] account from id: {accountId}, No of accounts {model.Count} ");
                _accountBalanceService.SeedBalances(accountId, model.Select(s => new SeedBalancesDTO
                {
                    AccountId = s.AccountId,
                    Amount = s.Amount,
                    RequestId = s.RequestId,
                    TrasnsactionId = s.TrasnsactionId
                }).ToList());
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError($"[SeedBalances Error1] Message: {ex.Message}");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[SeedBalances Error1] Message: {ex.Message}");
                return Ok(ex.Message, "0");
            }
        }

        [HttpGet]
        [Route("BalanceTypes/{id}")]
        [ProducesResponseType(typeof(List<int>), 200)]
        public IActionResult GetBalanceTypesByAccountId(int id, string language = "ar")
        {
            try
            {
                var balancesTypes = _accountBalanceService.GetBalanceTypesByAccountId(id, language);
                return Ok(balancesTypes);

            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message, "0");
            }
        }

        [HttpGet]
        [Route("Balances/{accountId}")]
        [ProducesResponseType(typeof(List<AccountBalancesModel>), 200)]
        public IActionResult GetBalanceByAccountId(int accountId, string language = "ar")
        {
            try
            {
                var accountBalances = _accountBalanceService.GetBalancesByAccountId(accountId, language).Select(x => new AccountBalancesModel
                {
                    TotalBalance = x.TotalBalance,
                    TotalAvailableBalance = x.TotalAvailableBalance,
                    BalanceType = x.BalanceType,
                    BalanceTypeId = x.BalanceTypeId
                }).ToList();

                return Ok(accountBalances);

            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message, "0");
            }
        }
    }
}
