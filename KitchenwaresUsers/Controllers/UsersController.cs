using KitchenwaresUsers.Models;
using KitchenwaresUsers.Services;
using Microsoft.AspNetCore.Mvc;

namespace KitchenwaresUsers.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpPut("{username:length(24)}")]
    public async Task<IActionResult> Update(string username, UserRequest request)
    {
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
        
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string username)
    {
        var user = await userService.FindOne(username);
        if (user == null)
        {
            return NotFound();
        }

        await userService.Delete(username);
        
        return NoContent();
    }
}