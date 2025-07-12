using System;
using System.Collections.Generic;

namespace AuthMicroservice.Infrastructure.Data;

public partial class LoginHistory
{
    public long HistoryId { get; set; }

    public Guid? UserId { get; set; }

    public string UsernameAttempt { get; set; } = null!;

    public DateTime LoginTimestamp { get; set; }

    public bool IsSuccess { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public virtual User? User { get; set; }
}
