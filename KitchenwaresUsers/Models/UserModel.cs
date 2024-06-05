using MongoDB.Bson.Serialization.Attributes;

namespace KitchenwaresUsers.Models;

public class UserModel
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? StoreName { get; set; } = null!;
}