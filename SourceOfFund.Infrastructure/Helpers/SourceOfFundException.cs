using System;
using System.Collections.Generic;
using System.Text;

namespace SourceOfFund.Infrastructure.Helpers
{
    public class SourceOfFundException : Exception
    {
        public string ErrorCode { get; private set; }
        public object ExceptionDate { get; set; }
        public SourceOfFundException() : base()
        {

        }

        public SourceOfFundException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
