﻿using System.Text;
using System.Text.Json;
using KitchenwaresUsers.Models;
using KitchenwaresUsers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenwaresUsers.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UsersController(IUserService userService, RabbitMqService rabbitMqService) : ControllerBase
{
    [Authorize(Roles = "User")]
    [HttpPut]
    public async Task<IActionResult> Update(UserRequest request)
    {
        var username = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")!.Value;
        
        var user = await userService.FindOne(username);
        if (user == null)
        {
            return NotFound();
        }

        var newUser = new UserModel
        {
            Username = username,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            StoreName = request.StoreName
        };
        await userService.Update(username, newUser);
        var authRequest = new UserAuthRequest
        {
            Mode = "UPDATE",
            Username = username,
            StoreName = request.StoreName
        };
        var jsonRequest = JsonSerializer.Serialize(authRequest);
        var body = Encoding.UTF8.GetBytes(jsonRequest);
        rabbitMqService.SendMessage(body);
        
        return NoContent();
    }

    [Authorize(Roles = "User")]
    [HttpDelete]
    public async Task<IActionResult> Delete()
    {
        var username = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")!.Value;
        
        var user = await userService.FindOne(username);
        if (user == null)
        {
            return NotFound();
        }

        await userService.Delete(username);
        var authRequest = new UserAuthRequest
        {
            Mode = "DELETE",
            Username = username
        };
        var jsonRequest = JsonSerializer.Serialize(authRequest);
        var body = Encoding.UTF8.GetBytes(jsonRequest);
        rabbitMqService.SendMessage(body);
        
        return NoContent();
    }
}