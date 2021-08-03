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

                _logger.LogInformation($"[Post] request id {requestId} with amount {model.Amount}, account id {accountId}");
                _accountBalanceService.HoldAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    Amount = model.Amount,
                    RequestId = requestId,
                    BalanceTypeId = balanceTypeId
                });
                
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, "[Post Exception]");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Post Exception]");
                return BadRequest(ex.Message, "0");
            }
            return Ok("Success", "200");
        }
        [HttpDelete]
        [Route("{accountId}/requests/{requestId}")]
        public IActionResult Refund([FromRoute] int accountId, int requestId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation($"[Refund] request id {requestId}, account id {accountId}");
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
                _logger.LogInformation($"[Confirm] request id {requestId}, account id {accountId}");
                _accountBalanceService.ConfirmAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                    TransactionIds = transactionIds.ToList()
                }); ;
            }
            catch (SourceOfFundException ex)
            {
                _logger.LogError(ex, "[Confirm Exception]");
                return Ok(ex.Message, ex.ErrorCode);
            }
            catch (DBConcurrencyException dbex)
            {
                _logger.LogError("[DBConcurrency Exception]");
                return Ok("Success", "200");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Confirm Exception]");
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
        [Route("ManageBalance/{fromAccountId}/{toAccountId}/balances/{amount}/{transactionType}")]
        public IActionResult ManageBalance(int fromAccountId, int toAccountId, decimal amount, TransactionType transactionType)
        {
            try
            {
                _accountBalanceService.ManageBalance(fromAccountId, toAccountId, amount);
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
        [Route("CreateAccount/{accountId}/balances/{amount}")]
        public IActionResult CreateAccount(int accountId, decimal amount)
        {
            try
            {
                _accountBalanceService.CreateAccount(accountId, amount);
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
    }
}
