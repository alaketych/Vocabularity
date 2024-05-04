using Microsoft.AspNetCore.Mvc;
using Vocabularity.Service.Dictionary.Interfaces;
using Vocabularity.Service.Dictionary.Entities;
using Vocabularity.Service.Language.Entities;
using Vocabularity.Service.Language.Implementation;
using Vocabularity.Service.Language.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;

namespace Vocabularity.API.Controllers;

[ApiController]
public class DictionaryController : ControllerBase
{
    private readonly IDictionaryRepository dictionaryRepository;

    public DictionaryController(IDictionaryRepository dictionaryRepository)
    {
        this.dictionaryRepository = dictionaryRepository;
    }

    [HttpGet("{id}/{partitionKey}")]
    public async Task<IActionResult> GetLanguage(string id, string partitionKey)
    {
        var language = await dictionaryRepository.GetByIdAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok(language);
    }

    [HttpGet("dictionaries")]
    public async Task<List<Dictionary>> GetDictionaries()
    {
        var result = new List<Dictionary>();
        await foreach (var item in dictionaryRepository.GetAllAsync())
        { 
            result.Add(item);
        }

        return result;
    }

    [HttpPost("dictionary")]
    public async Task<IActionResult> CreateLanguage(Dictionary dictionary)
    {
        await dictionaryRepository.CreateAsync(dictionary, new PartitionKey(dictionary.Name).ToString());
        return Ok();
    }

    [HttpPut("dictionary")]
    public async Task<IActionResult> UpdateLanguage(Dictionary dictionary)
    {
        await dictionaryRepository.UpdateAsync(dictionary, new PartitionKey(dictionary.Name).ToString());
        return Ok();
    }

    [HttpDelete("{id}/{partitionKey}")]
    public async Task<IActionResult> DeleteAsync(string id, string partitionKey)
    {
        await dictionaryRepository.DeleteAsync(id, new PartitionKey(partitionKey).ToString());
        return Ok();
    }
}
