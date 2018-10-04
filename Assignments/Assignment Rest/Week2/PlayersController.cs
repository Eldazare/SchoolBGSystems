using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Week2
{

    [Route("api/[controller]")]
    [ApiController]
    [InvalidPlayerIDFilter]
    public class PlayersController : Controller
    {
        PlayersProcessor processor;
        public PlayersController(PlayersProcessor proc){
            processor = proc;
        }
        /*
        [HttpGet]
        public ActionResult<string> TestCall(){
            return "Test complete";
        }
        */
        [HttpGet("{id:Guid}")]
        public Task<Player> Get(Guid id){
            return processor.Get(id);
        }

        [HttpGet("{name}")]
        public Task<Player> GetByName(string name){
            return Task.FromResult(new Player());
        }
        [HttpGet]
        public Task<Player[]> GetAll([FromQuery] int? minScore, [FromQuery] PlayerTag tag, 
                [FromQuery] string itemType, [FromQuery] int? itemAmount){
            if (minScore != null){
                return processor.GetAllWithScore(minScore);
            } else if (tag != PlayerTag.None){
                return processor.GetAllWithTag(tag);
            } else if (!string.IsNullOrEmpty(itemType)){
                return processor.GetAllWithItem(itemType);
            } else if (itemAmount != null){
                return processor.GetAllWithAmountOfItems((int)itemAmount);
            }else {
                return processor.GetAll();
            }
        }
        [HttpGet("{CommonLevel}")]
        public Task<String> GetAverageLevel(){
            return processor.GetAverageLevel();
        }

        [HttpGet("AverageScore")]
        public Task<String> GetAverageScoreBetweenDates([FromQuery] DateTime startDate, [FromQuery] DateTime endDate){
            return processor.GetAverageScorePerDates(startDate, endDate);
        }

        [HttpPost]
        public Task<Player> Create(NewPlayer player){
            return processor.Create(player);
        }

        [HttpPut("{id}")]
        public Task<Player> Modify(Guid id, ModifiedPlayer player){
            return processor.Modify(id, player);
        }

        [HttpGet("ban/{id:guid}")]
        [Authorize(Roles = "Admin")]
        [RepoAuditFilter]
        public Task<Player> BanPlayer(Guid id){
            return processor.BanPlayer(id);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public Task<Player> Delete(Guid id){
            return processor.Delete(id);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public Task<bool> DeleteAll(){
            return processor.DeleteAll();
        }

        [HttpGet("logs")]
        public Task<string> GetLogs(){
            return processor.GetLogs();
        }
    }
}