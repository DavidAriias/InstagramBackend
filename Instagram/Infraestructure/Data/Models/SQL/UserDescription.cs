using System;
using System.Collections.Generic;

namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class UserDescription
{
    public Guid UserId { get; set; }

    public bool Isverificated { get; set; }

    public string? Imageprofile { get; set; }

    public string? Description { get; set; }

    public bool Isprivated { get; set; }

    public string? Pronoun { get; set; }

    public Guid Id { get; set; }

    public virtual UserDatum User { get; set; } = null!;
}
