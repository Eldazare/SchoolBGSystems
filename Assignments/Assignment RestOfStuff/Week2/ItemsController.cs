using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Week2
{
    [Route("api/players/{playerId}/[controller]")]
    [ApiController]
    [InvalidPlayerIDFilter]
    public class ItemsController : ControllerBase
    {
        ItemsProcessor processor;

        public ItemsController(ItemsProcessor processo){
            processor = processo;
        }

        [HttpGet]
        public Task<List<Item>> GetItems(Guid playerId){
            return processor.GetAllItems(playerId);
        }

        [HttpGet("{itemPos}")]
        public Task<Item> GetAnItem(Guid playerId, int itemPos){
            return processor.GetAnItem(playerId, itemPos);
        }

        [HttpPost]
        [InvalidItemExceptionFilter]
        public Task<Item> AddAnItem(Guid playerId, NewItem newItem){
            return processor.AddItem(playerId, newItem);
        }

        [HttpPut("{itemPos}")]
        [InvalidItemExceptionFilter]
        public Task<Item> ModifyItem(Guid playerId, int itemPos, ModifiedItem modItem){
            return processor.ModifyItem(playerId, itemPos, modItem);
        }

        [HttpDelete("{itemPos}")]
        public Task<bool> DeleteAnItem(Guid playerId, int itemPos){
            return processor.DeleteItem(playerId, itemPos);
        }
    }
}