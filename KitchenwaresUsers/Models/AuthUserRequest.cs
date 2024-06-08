namespace KitchenwaresUsers.Models;

public class AuthUserRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? StoreName { get; set; } = null!;
}