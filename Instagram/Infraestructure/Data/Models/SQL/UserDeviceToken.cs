using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class UserDeviceToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string DeviceToken { get; set; } = null!;

    public string DeviceType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual UserDatum User { get; set; } = null!;
}
