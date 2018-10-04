using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Week2
{
    public class InMemoryRepository //:IRepository
    {
        List<Player> memory = new List<Player>();

        public Task<Player> Get(Guid id){
            Player player = Find(id);
            return Task.FromResult(player);
        }

        private Player Find(Guid id){
            foreach (Player player in memory){
                if (player.Id == id){
                    return player;
                }
            }
            return null;
        }

        public Task<Player[]> GetAll(){
            Player[] returnee = memory.ToArray();
            return Task.FromResult(returnee);
        }

        public Task<Player> Create(Player player){
            memory.Add(player);
            return Task.FromResult(player);
        }
        public Task<Player> Modify(Guid id, ModifiedPlayer player){
            Player playerFound = Find(id);
            playerFound.Score = (int)player.Score;
            return Task.FromResult(playerFound);
        }

        public Task<Player> Replace(Player player){
            Delete(player.Id);
            Create(player);
            return Task.FromResult(player);
        }
        public Task<Player> Delete(Guid id){
            Player player = Find(id);
            memory.Remove(player);
            return Task.FromResult(player);
        }

        public Task<Boolean> DeleteAll(){
            memory = new List<Player>();
            return Task.FromResult(true);
        }
    }
}