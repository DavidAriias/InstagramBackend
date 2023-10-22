using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class UserName
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime Updatedate { get; set; }

    public Guid Id { get; set; }

    public virtual UserDatum User { get; set; } = null!;
}
