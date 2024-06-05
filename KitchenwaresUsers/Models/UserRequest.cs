namespace KitchenwaresUsers.Models;

public class UserRequest
{
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? StoreName { get; set; } = null!;
}