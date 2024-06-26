﻿using KitchenwaresUsers.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KitchenwaresUsers.Services;

public interface IUserService
{
    Task<List<UserModel>> FindAll();
    Task<UserModel?> FindOne(string username);
    Task Create(AuthUserRequest authUser);
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

    public async Task<List<UserModel>> FindAll() => await _users.Find(_ => true).ToListAsync();
    
    public async Task<UserModel?> FindOne(string username) =>
        await _users.Find(x => x.Username == username).FirstOrDefaultAsync();

    public async Task Create(AuthUserRequest authUser)
    {
        var newUser = new UserModel
        {
            Username = authUser.Username,
            Email = authUser.Email,
            PhoneNumber = authUser.PhoneNumber,
            StoreName = authUser.StoreName
        };
        await _users.InsertOneAsync(newUser);
    }
    
    public async Task Update(string username, UserModel newUser)
    {
        await _users.ReplaceOneAsync(x => x.Username == username, newUser);
    }

    public async Task Delete(string username)
    {
        await _users.DeleteOneAsync(x => x.Username == username);
    }
}