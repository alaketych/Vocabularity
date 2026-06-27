using Microsoft.AspNetCore.Mvc;
using Vocabularity.Core.Entities;
using Vocabularity.Service.Dictionary.Interfaces;

namespace Vocabularity.API.Controllers;

[ApiController]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryRepository dictionaryRepository;

    public DictionaryController(IDictionaryRepository dictionaryRepository)
    {
        this.dictionaryRepository = dictionaryRepository;
    }

    [HttpGet("dictionary/{id}")]
    public async Task<IActionResult> GetDictionary(string id)
    {
        var dictionary = await dictionaryRepository.GetByIdAsync(id);
        if (dictionary is null)
        {
            return NotFound();
        }

        return Ok(dictionary);
    }

    [HttpGet("dictionaries")]
    public async Task<List<DictionaryEntry>> GetDictionaries()
    {
        var result = new List<DictionaryEntry>();
        await foreach (var item in dictionaryRepository.GetAllAsync())
        {
            result.Add(item);
        }

        return result;
    }

    [HttpPost("dictionary")]
    public async Task<IActionResult> CreateDictionary(DictionaryEntry dictionary)
    {
        await dictionaryRepository.CreateAsync(dictionary);
        return Ok(dictionary);
    }

    [HttpPut("dictionary")]
    public async Task<IActionResult> UpdateDictionary(DictionaryEntry dictionary)
    {
        await dictionaryRepository.UpdateAsync(dictionary);
        return Ok();
    }

    [HttpDelete("dictionary/{id}")]
    public async Task<IActionResult> DeleteDictionary(string id)
    {
        await dictionaryRepository.DeleteAsync(id);
        return Ok();
    }
}
