using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public class AccountsController : ControllerBase
    {
        private readonly IAccountBalanceService _accountBalanceService;
        public AccountsController(IAccountBalanceService accountBalanceService)
        {
            _accountBalanceService = accountBalanceService;
        }

        [HttpGet]
        [Route("GetBalance/{AccountID}/{balanceTypeID}")]
        public IActionResult GetBalance(int accountID, int balanceTypeID, string language = "ar")
        {
            try
            {
                var balancesModel = _accountBalanceService.GetBalance(accountID, balanceTypeID, language);
                return Ok(new AccountBalanceResponseModel { Code = 200, TotalBalance = balancesModel.TotalBalance, TotalAvailableBalance = balancesModel.TotalAvailableBalance });

            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Helper Method
        #region Helper Method

        //private AccountRequestModel Map(AccountBalanceDTO model)
        //{
        //    return new AccountRequestModel
        //    {
        //        Id = model.Id,
        //        OwnerName = model.OwnerName,
        //        AccountName = model.AccountName,
        //        Mobile = model.Mobile,
        //        Address = model.Address,
        //        Email = model.Email,
        //        NationalID = model.NationalID,
        //        CommercialRegistrationNo = model.CommercialRegistrationNo,
        //        TaxNo = model.TaxNo,
        //        ActivityID = model.ActivityID,
        //        ActivityName = model.ActivityName
        //    };
        //} 
        #endregion
    }
}
