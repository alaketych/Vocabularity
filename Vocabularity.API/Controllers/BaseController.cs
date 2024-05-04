using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Vocabularity.Core;
using Vocabularity.Core.Interfaces;
using Vocabularity.Service.Language.Entities;
using Vocabularity.Service.Language.Implementation;
using Vocabularity.Service.Language.Interfaces;

namespace Vocabularity.API.Controllers
{
    [ApiController]
    public class BaseController<TEntity> : ControllerBase
        where TEntity : Entity
    {
        private readonly IRepository<TEntity> repository;
    
        public BaseController(IRepository<TEntity> repository)
        {
            this.repository = repository;
        }

        [HttpGet("{id}/{partitionKey}")]
        public async Task<IActionResult> GetById(string id, string partitionKey)
        {
            var language = await repository.GetByIdAsync(id, new PartitionKey(partitionKey).ToString());
            return Ok(language);
        }

        [HttpGet]
        public async Task<IActionResult> GetLanguages()
        {
            var result = new List<TEntity>();
            await foreach (var item in repository.GetAllAsync())
            {
                result.Add(item);
            }

            return Ok(result);
        }

        [HttpPost("language")]
        public async Task<IActionResult> CreateLanguage(TEntity entity)
        {
            await repository.CreateAsync(entity, new PartitionKey(entity.Id).ToString());
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateLanguage(TEntity entity)
        {
            await repository.UpdateAsync(entity, new PartitionKey(entity.Id).ToString());
            return Ok();
        }

        [HttpDelete("{id}/{partitionKey}")]
        public async Task<IActionResult> DeleteAsync(string id, string partitionKey)
        {
            await repository.DeleteAsync(id, new PartitionKey(partitionKey).ToString());
            return Ok();
        }
    }
}
