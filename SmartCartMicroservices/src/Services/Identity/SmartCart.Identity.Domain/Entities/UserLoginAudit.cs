using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCart.Identity.Domain.Entities;

public class UserLoginAudit
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public bool IsSuccessful { get; set; }

    public string? FailureReason { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime LoginAt { get; set; }

    public User? User { get; set; }
}
