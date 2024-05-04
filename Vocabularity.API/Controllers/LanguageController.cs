using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using Vocabularity.Service.Language.Interfaces;
using Vocabularity.Service.Language.Entities;

namespace Vocabularity.API.Controllers;

[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepository languageRepository;

    public LanguageController(ILanguageRepository languageRepository)
    {
        this.languageRepository = languageRepository;
    }

    [HttpGet("{id}/{partitionKey}")]
    public async Task<IActionResult> GetLanguage(string id, string partitionKey)
    {
        var language = await languageRepository.GetByIdAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok(language);
    }

    [HttpGet("languages")]
    public async Task<IActionResult> GetLanguages()
    {
        var result = new List<Language>();
        await foreach (var item in languageRepository.GetAllAsync())
        {
            result.Add(item);
        }

        return Ok(result);
    }

    [HttpPost("language")]
    public async Task<IActionResult> CreateLanguage(Language language)
    {
        await languageRepository.CreateAsync(language, new PartitionKey(language.Name).ToString());
        return Ok();
    }

    [HttpPut("language")]
    public async Task<IActionResult> UpdateLanguage(Language language)
    {
        await languageRepository.UpdateAsync(language, new PartitionKey(language.Name).ToString());
        return Ok();
    }

    [HttpDelete("{id}/{partitionKey}")]
    public async Task<IActionResult> DeleteAsync(string id, string partitionKey)
    { 
        await languageRepository.DeleteAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok();
    }
}
