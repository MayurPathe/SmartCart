using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Application.Commands
{
    public class LogoutCommand
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
