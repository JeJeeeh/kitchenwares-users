namespace KitchenwaresUsers.Models;

public class UserAuthRequest
{
    public string Mode { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? StoreName { get; set; } = null!;
}