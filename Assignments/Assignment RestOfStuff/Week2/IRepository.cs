using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace Week2
{
    public interface IRepository {
        Task<Player> Get(Guid id);
        Task<Player[]> GetAll();

        Task<Player[]> GetAllWithScore(int? minScore);
        Task<Player[]> GetAllWithTag(PlayerTag tag);

        Task<Player[]> GetAllWithItem(string itemType);

        Task<Player[]> GetAllWithAmountOfItems(int itemAmount);
        Task<Player> Create(Player player);
        Task<Player> Modify(Guid id, ModifiedPlayer player);
        Task<Player> Replace(Player player);
        Task<Player> Delete(Guid id);
        Task<bool> DeleteAll();

        Task<Item> PushItem(Guid id, Item item);
        Task<String> GetCommonLevelBetweenPlayers();
        Task<String> GetAverageScorePerDates(DateTime startTime, DateTime endTime);
    }
}