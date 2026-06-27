using Microsoft.AspNetCore.Mvc;
using Vocabularity.Core.Entities;
using Vocabularity.Service.Language.Interfaces;

namespace Vocabularity.API.Controllers;

[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILanguageRepository languageRepository;

    public LanguageController(ILanguageRepository languageRepository)
    {
        this.languageRepository = languageRepository;
    }

    [HttpGet("language/{id}")]
    public async Task<IActionResult> GetLanguage(string id)
    {
        var language = await languageRepository.GetByIdAsync(id);
        if (language is null)
        {
            return NotFound();
        }

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
        await languageRepository.CreateAsync(language);
        return Ok(language);
    }

    [HttpPut("language")]
    public async Task<IActionResult> UpdateLanguage(Language language)
    {
        await languageRepository.UpdateAsync(language);
        return Ok();
    }

    [HttpDelete("language/{id}")]
    public async Task<IActionResult> DeleteLanguage(string id)
    {
        await languageRepository.DeleteAsync(id);
        return Ok();
    }
}
