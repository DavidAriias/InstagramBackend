using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class UserDatum
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string? Email { get; set; }

    public string Password { get; set; } = null!;

    public string? Phonenumber { get; set; }

    public virtual ICollection<Birthday> Birthdays { get; set; } = new List<Birthday>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<UserDescription> UserDescriptions { get; set; } = new List<UserDescription>();

    public virtual ICollection<UserDeviceToken> UserDeviceTokens { get; set; } = new List<UserDeviceToken>();

    public virtual ICollection<UserName> UserNames { get; set; } = new List<UserName>();
}
