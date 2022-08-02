using System;
using System.Collections.Generic;

namespace Dynamics365API.Dtos
{
    public class AuthDto
    {
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }

    }
}