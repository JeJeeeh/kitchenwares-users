using KitchenwaresUsers.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KitchenwaresUsers.Services;

public interface IUserService
{
    Task<UserModel?> FindOne(string username);
    Task Update(string username, UserModel newUser);
    Task Delete(string username);
}

public class UserService : IUserService
{
    private readonly IMongoCollection<UserModel> _users;
    
    public UserService(IOptions<DatabaseSettings> databaseSettings) 
    {
        var mongoClient = new MongoClient(
            databaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            databaseSettings.Value.DatabaseName);

        _users = mongoDatabase.GetCollection<UserModel>(
            databaseSettings.Value.CollectionName);
    }

    public async Task<UserModel?> FindOne(string username) =>
        await _users.Find(x => x.Username == username).FirstOrDefaultAsync();
    
    public async Task Update(string username, UserModel newUser)
    {
        await _users.ReplaceOneAsync(x => x.Username == newUser.Username, newUser);
    }

    public async Task Delete(string username)
    {
        await _users.DeleteOneAsync(x => x.Username == username);
    }
}