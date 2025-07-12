using System;
using System.Collections.Generic;

namespace AuthMicroservice.Infrastructure.Data;

public partial class RefreshToken
{
    public long RefreshTokenId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
