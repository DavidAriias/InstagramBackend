using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class UserLink
{
    public Guid UserId { get; set; }

    public string Link { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual UserDatum User { get; set; } = null!;
}
