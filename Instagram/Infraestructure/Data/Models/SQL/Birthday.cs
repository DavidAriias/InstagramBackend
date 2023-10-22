using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class Birthday
{
    public Guid UserId { get; set; }

    public DateOnly Birthdaydate { get; set; }

    public DateTime Updatedate { get; set; }

    public Guid Id { get; set; }

    public virtual UserDatum User { get; set; } = null!;
}
