using Microsoft.AspNetCore.Mvc;
using Vocabularity.Core.Entities;
using Vocabularity.Service.User.Interfaces;

namespace Vocabularity.API.Controllers;

[ApiController]
public class UserController : Controller
{
    private readonly IUserRepository userRepository;

    public UserController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpGet("users")]
    public async Task<List<User>> GetUsers()
    {
        var result = new List<User>();
        await foreach (var item in userRepository.GetAllAsync())
        {
            result.Add(item);
        }

        return result;
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateUser(User user)
    {
        await userRepository.CreateAsync(user);
        return Ok(user);
    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateUser(User user)
    {
        await userRepository.UpdateAsync(user);
        return Ok();
    }

    [HttpDelete("user/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        await userRepository.DeleteAsync(id);
        return Ok();
    }
}
