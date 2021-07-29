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

namespace SourceOfFund.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountsController : BaseController
    {
        private readonly IAccountBalanceService _accountBalanceService;
        public AccountsController(IAccountBalanceService accountBalanceService)
        {
            _accountBalanceService = accountBalanceService;
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
                    BalanceTypeId = balanceTypeId
                });
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
                }); ;
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
        [Route("{fromAccountId}/{toAccountId}/requests/{requestId}/")]
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
                _accountBalanceService.ManageBalance(fromAccountId, toAccountId, amount,transactionType);
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
