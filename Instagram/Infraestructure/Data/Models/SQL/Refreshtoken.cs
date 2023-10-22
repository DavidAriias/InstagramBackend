namespace Instagram.Infraestructure.Data.Models.SQL;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string TokenValue { get; set; } = null!;

    public virtual UserDatum User { get; set; } = null!;
}
