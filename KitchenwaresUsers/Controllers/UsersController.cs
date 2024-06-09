using System.Text;
using System.Text.Json;
using KitchenwaresUsers.Models;
using KitchenwaresUsers.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KitchenwaresUsers.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UsersController(IUserService userService, IRabbitMqService rabbitMqService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<UserModel>> FindAll()
    {
        var users = await userService.FindAll();
        return Ok(users);
    }
    
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
        rabbitMqService.SendMessage(authRequest);
        
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
        rabbitMqService.SendMessage(authRequest);
        
        return NoContent();
    }
}