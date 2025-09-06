using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ovation.Application.DTOs
{
    public class UserToken
    {
        public object? UserData { get; set; } = null;
        public string? Token { get; set; } = null;
        public Guid UserId { get; set; } = Guid.Empty;
        public string Message { get; set; } = "Success";
    }
}
