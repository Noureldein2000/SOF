﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SourceOfFund.API.Models;
using SourceOfFund.Infrastructure.Helpers;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Services.Models;
using SourceOfFund.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        [Route("{accountId}/services/{balanceTypeId}/balances")]
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
                return BadRequest(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("0", "");
            }
        }


        [HttpPost]
        [Route("{accountId}/services/{balanceTypeId}/requests/{requestId}")]
        public IActionResult Post([FromBody] HoldBalanceModel model, int accountId, int balanceTypeId, int requestId)
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
                return Ok("200", "");
            }
            catch (SourceOfFundException ex)
            {
                return BadRequest(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("0", "");
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
                return Ok("200", "");
            }
            catch (SourceOfFundException ex)
            {
                return BadRequest(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("0", "");
            }
        }

        [HttpPut]
        [Route("{accountId}/services/{balanceTypeId}/requests/{requestId}")]
        public IActionResult Confirm([FromBody] HoldBalanceModel model, int accountId, int balanceTypeId, int requestId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _accountBalanceService.ConfirmAmount(new HoldBalanceDTO
                {
                    AccountId = accountId,
                    RequestId = requestId,
                    Amount = model.Amount,
                    BalanceTypeId = balanceTypeId
                });
                return Ok("200", "");
            }
            catch (SourceOfFundException ex)
            {
                return BadRequest(ex.ErrorCode, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest("0", "");
            }
        }

    }
}
