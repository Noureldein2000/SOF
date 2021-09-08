using SourceOfFund.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Services.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
        ApplicationDbContext GetContext();
    }
}
