using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Application.Common.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; } = 400;

        public AppException(string message)
            : base(message)
        {
        }
    }
}
