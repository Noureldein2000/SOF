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
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("{accountId}/balances/{balanceTypeId}/requests/{requestId}")]
        public IActionResult Post([FromBody] HoldBalanceModel model, int accountId, int requestId, int? balanceTypeId = null)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _accountBalanceService.HoldAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    Amount = model.Amount,
                    RequestId = requestId,
                    BalanceTypeId = balanceTypeId.HasValue ? balanceTypeId.Value : 1
                });
                _logger.LogInformation($"[Hold] request id: {requestId}, account id: {accountId}, amount: {model.Amount}");
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
                return BadRequest(dbex.Message, "0");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Post Exception] {ex.Message}");
                return BadRequest(ex.Message, "0");
            }
        }
        [HttpDelete]
        [Route("{accountId}/requests/{requestId}")]
        public IActionResult Refund([FromRoute] int accountId, int requestId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);


                _accountBalanceService.RefundAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                });
                _logger.LogInformation($"[Refund] request id: {requestId}, account id: {accountId}");
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
                return BadRequest(ex.Message, "0");
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
                    return BadRequest(ModelState);
                _accountBalanceService.ConfirmAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                    TransactionIds = transactionIds.ToList()
                });
                _logger.LogInformation($"[Confirmed] request id: {requestId}, account id: {accountId}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
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
                _accountBalanceService.ReturnBalance(fromAccountId, toAccountId, amount);
                _logger.LogInformation($"[ReturnBalance] account from id: {fromAccountId}, account to id: {toAccountId} amount: {amount}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
            }
        }

        [HttpPost]
        [Route("ConfirmTransfer/{fromAccountId}/{toAccountId}/requests/{requestId}")]
        public IActionResult ConfirmTransfer(int fromAccountId, int toAccountId, int requestId)
        {
            try
            {
                _accountBalanceService.ConfirmTransfer(fromAccountId, toAccountId, requestId);
                _logger.LogInformation($"[ConfirmTransfer] account from id: {fromAccountId}, account to id: {toAccountId} request id: {requestId}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
            }
        }

        [HttpPost]
        [Route("ManageBalance/{fromAccountId}/{toAccountId}/balances/{amount}/requests/{accountFromRequestId}/transactions/{accountFromTransactionId}")]
        public IActionResult ManageBalance(int fromAccountId, int toAccountId, decimal amount, int accountFromRequestId, int accountFromTransactionId)
        {
            try
            {
                _accountBalanceService.ManageBalance(fromAccountId, toAccountId, amount, accountFromRequestId, accountFromTransactionId);
                _logger.LogInformation($"[ManageBalance] account from id: {fromAccountId}, account to id: {toAccountId} amount: {amount}, requestId: {accountFromRequestId}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
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
                _accountBalanceService.ManageBalance(model.FromAccountId, model.ToAccountId, model.Amount, model.RequestId, model.TransactionId, accountTypeId: model.BalanceTypeId);
                _logger.LogInformation($"[ManageBalance] account from id: {model.FromAccountId}, account to id: {model.ToAccountId} amount: {model.Amount}, requestId: {model.RequestId}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
            }
        }
        [HttpPost]
        [Route("CreateAccount")]
        public IActionResult CreateAccount([FromBody] CreateAccountModel model)
        {
            try
            {
                _accountBalanceService.CreateAccount(model.AccountId, model.Amount, model.BalanceTypeIds);
                _logger.LogInformation($"[CreateAccount] account id: {model.AccountId}, amount: {model.Amount}, balanceType: {model.BalanceTypeIds}");
                return Ok("Success", "200");
            }
            catch (SourceOfFundException ex)
            {
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message, "0");
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
                return BadRequest(ex.Message, "0");
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
                return BadRequest(ex.Message, "0");
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
                return BadRequest(ex.Message, "0");
            }
        }
    }
}
