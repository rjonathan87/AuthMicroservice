using System;
using System.Collections.Generic;

namespace AuthMicroservice.Infrastructure.Data;

public partial class JwtToken
{
    public long TokenId { get; set; }

    public string TokenIdentifier { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public DateTime RevokedAt { get; set; }

    public Guid UserId { get; set; }

    public string? Reason { get; set; }

    public virtual User User { get; set; } = null!;
}
