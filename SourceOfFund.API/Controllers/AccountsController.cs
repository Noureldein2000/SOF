using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    }
}
