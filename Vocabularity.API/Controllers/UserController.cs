using Microsoft.AspNetCore.Mvc;
using Vocabularity.Service.User.Interfaces;
using Vocabularity.Service.User.Entities;
using Vocabularity.Service.Language.Implementation;
using Microsoft.Azure.Cosmos;
using Vocabularity.Service.Language.Entities;
using Vocabularity.Service.Language.Interfaces;
using Vocabularity.Service.User.Implementation;

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
    public async Task<List<Service.User.Entities.User>> GetUsers()
    { 
        var result = new List<Service.User.Entities.User>();
        await foreach (var item in userRepository.GetAllAsync())
        {
            result.Add(item);
        }

        return result;
    }

    [HttpGet("{id}/{partitionKey}")]
    public async Task<IActionResult> GetLanguage(string id, string partitionKey)
    {
        var language = await userRepository.GetByIdAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok(language);
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = new List<Service.User.Entities.User>();
        await foreach (var item in userRepository.GetAllAsync())
        {
            result.Add(item);
        }

        return Ok(result);
    }

    [HttpPost("user")]
    public async Task<IActionResult> CreateLanguage(Service.User.Entities.User user)
    {
        await userRepository.CreateAsync(user, new PartitionKey(user.Pseudonym).ToString());
        return Ok();
    }

    [HttpPut("user")]
    public async Task<IActionResult> UpdateLanguage(Service.User.Entities.User user)
    {
        await userRepository.UpdateAsync(user, new PartitionKey(user.Pseudonym).ToString());
        return Ok();
    }

    [HttpDelete("{id}/{partitionKey}")]
    public async Task<IActionResult> DeleteAsync(string id, string partitionKey)
    {
        await userRepository.DeleteAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok();
    }
}
