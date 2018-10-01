using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Week2
{
    public class PlayersProcessor
    {
        IRepository repository;
        public PlayersProcessor(IRepository rep){
            repository = rep;
        }
        
        public Task<Player> Get(Guid id){
            return repository.Get(id);
        }
        public Task<Player[]> GetAll(){
            return repository.GetAll();
        }

        public Task<Player[]> GetAllWithScore(int? minScore){
            return repository.GetAllWithScore(minScore);
        }

        public Task<Player[]> GetAllWithTag(PlayerTag tag){
            return repository.GetAllWithTag(tag);
        }
        public Task<Player[]> GetAllWithItem(string itemType){
            return repository.GetAllWithItem(itemType);
        }
        public Task<Player[]> GetAllWithAmountOfItems(int itemAmount){
            return repository.GetAllWithAmountOfItems(itemAmount);
        }
        public Task<Player> Create(NewPlayer player){
            Player createdPlayer = new Player();
            createdPlayer.Name = player.Name;
            createdPlayer.Id = Guid.NewGuid();
            createdPlayer.IsBanned = false;
            createdPlayer.Level = 1;
            createdPlayer.Score = 0;
            createdPlayer.CreationTime = DateTime.UtcNow;
            createdPlayer.Inventory = new List<Item>();
            createdPlayer.TagList = new List<PlayerTag>();
            return repository.Create(createdPlayer);
        }
        public Task<Player> Modify(Guid id, ModifiedPlayer player){
            return repository.Modify(id, player);
        }
        public Task<Player> Delete(Guid id){
            return repository.Delete(id);
        }

        public Task<bool> DeleteAll(){
            return repository.DeleteAll();
        }

        public Task<String> GetAverageLevel(){
            return repository.GetCommonLevelBetweenPlayers();
        }

        public Task<string> GetAverageScorePerDates(DateTime startTime, DateTime endTime){
            return repository.GetAverageScorePerDates(startTime, endTime);
        }
    }
}