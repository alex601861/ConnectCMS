using CMSTrain.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CMSTrain.Domain.Entities;

[Owned]
public class RefreshToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Revoked { get; set; }

    public bool IsActive => Revoked == null && !IsExpired;

    private bool IsExpired => DateTime.Now >= Expires;
}