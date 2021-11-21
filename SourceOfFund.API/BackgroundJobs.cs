using Microsoft.Extensions.Configuration;
using SourceOfFund.Services.DTOs;
using SourceOfFund.Services.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SourceOfFund.API
{
    public class BackgroundJobs : IBackgroundJobs
    {
        private readonly IAccountBalanceService _accountBalanceService;
        private readonly IConfiguration _configuration;
        public BackgroundJobs(IAccountBalanceService accountBalanceService, IConfiguration configuration)
        {
            _accountBalanceService = accountBalanceService;
            _configuration = configuration;
        }
        public void AddCommissions()
        {
            var commissions = GetCommissions().ToList();
            _accountBalanceService.AddCommission(commissions);
        }

        private IEnumerable<AccountCommissionDTO> GetCommissions()
        {
            var connectionString = _configuration.GetConnectionString("OldServiceConnection");
            var list = new List<AccountCommissionDTO>();
            var dataTable = new DataTable();
            var cmd = new SqlCommand("GetCommissionForSourceOfFund", new SqlConnection(connectionString));
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@dateFrom", SqlDbType.Date).Value = DateTime.Today.AddDays(-1).ToShortDateString();
            cmd.Parameters.Add("@dateTo", SqlDbType.Date).Value = DateTime.Today.ToShortDateString();
            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dataTable);

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                list.Add(new AccountCommissionDTO
                {
                    TransactionId = int.Parse(dataTable.Rows[i][0].ToString()),
                    RequestId = int.Parse(dataTable.Rows[i][1].ToString()),
                    AccountId = int.Parse(dataTable.Rows[i][2].ToString()),
                    Amount = decimal.Parse(dataTable.Rows[i][3].ToString()),
                });
            }

            return list;
        }
    }
}
