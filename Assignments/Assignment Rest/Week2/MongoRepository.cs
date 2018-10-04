using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace Week2
{
    public class MongoRepository : IRepository
    {
        private string address = "mongodb://localhost:27017";
        private IMongoCollection<Player> Collection;
        private IMongoCollection<Log> LogCollection;
        private IMongoCollection<BsonDocument> DocumentsCollection;
        public MongoRepository(){
            var Client = new MongoClient(address);
            IMongoDatabase Database = Client.GetDatabase("NotGame");
            Collection = Database.GetCollection<Player>("players");
            DocumentsCollection = Database.GetCollection<BsonDocument>("players");

            LogCollection = Database.GetCollection<Log>("logs");
        }

        public Task<Player> Get(Guid id){
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("_id", id);
            var returned = Collection.Find(filter);
            if (returned.CountDocuments() == 0){
                throw new InvalidPlayerIDException(id.ToString());
            } else{
                return returned.FirstAsync();
            }
        }

        public async Task<Player[]> GetAll() {
            return (await Collection.Find(_=>true).ToListAsync()).ToArray();
        }

        public async Task<Player[]> GetAllWithScore(int? minScore){
            FilterDefinition<Player> filter = Builders<Player>.Filter.Gte("Score", minScore);
            var returned = Collection.Find(filter);
            return (await returned.ToListAsync()).ToArray();
        }
        public async Task<Player[]> GetAllWithTag(PlayerTag tag){
            FilterDefinition<Player> filter = Builders<Player>.Filter.ElemMatch(x => x.TagList, a => a == tag);
            var result = Collection.Find(filter);
            return (await result.ToListAsync()).ToArray();
        }

        public async Task<Player[]> GetAllWithItem(string itemType){
            FilterDefinition<Player> filter = Builders<Player>.Filter.ElemMatch(x => x.Inventory, a => a.Type == itemType);
            var result = Collection.Find(filter);
            return (await result.ToListAsync()).ToArray();
        }

        public async Task<Player[]> GetAllWithAmountOfItems(int itemAmount){
            FilterDefinition<Player> filter = Builders<Player>.Filter.Size("Inventory", itemAmount);
            var result = Collection.Find(filter);
            return (await result.ToListAsync()).ToArray();
        }
        public async Task<Player> Create(Player player){
            await Collection.InsertOneAsync(player);
            return player;
        }
        public async Task<Player> Modify(Guid id, ModifiedPlayer player){
            // Update, 2 documents, {$set : {querydoc}}
            Player modPlayer = new Player();
            UpdateDefinition<Player> updateList = Builders<Player>.Update.Set("_id",id);
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("_id", id);
            bool done = false;
            modPlayer.Id = id;
            if (player.Score != null){
                modPlayer.Score = (int)player.Score;
                updateList.Set("Score", modPlayer.Score); // DOESN'T WORK
                UpdateResult result = await Collection.UpdateOneAsync(filter, Builders<Player>.Update.Set("Score", modPlayer.Score));
                if (result.IsAcknowledged){ 
                    done = true;
                }
            }
            if (player.Level != null){
                modPlayer.Level = (int)player.Level;
                UpdateResult result = await Collection.UpdateOneAsync(filter, Builders<Player>.Update.Set("Level", modPlayer.Level));
                if (result.IsAcknowledged){
                    done = true;
                }

            }
            if (player.Tags != null){
                modPlayer.TagList = new List<PlayerTag>();
                foreach (PlayerTag tag in player.Tags){
                    modPlayer.TagList.Add(tag);
                }
                UpdateResult result = await Collection.UpdateOneAsync(filter, Builders<Player>.Update.Set("TagList", modPlayer.TagList));
                if (result.IsAcknowledged){
                    done = true;
                }
            }
            
            //UpdateResult result = await Collection.UpdateOneAsync(filter, updateList);
            if (done){
                return await Get(id);
            } else{
                throw new InvalidPlayerIDException(id.ToString());
            }
        }

        public async Task<Player> Replace(Player player){
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("_id", player.Id);
            ReplaceOneResult result = await Collection.ReplaceOneAsync(filter, player);
            if (result.IsAcknowledged){
                return player;
            } else {
                throw new InvalidPlayerIDException(player.Id.ToString());
            }
        }

        public async Task<Player> BanPlayer (Guid id){
            var filter = Builders<Player>.Filter.Eq("_id", id);
            var update = Builders<Player>.Update.Set("IsBanned", true);
            var result = await Collection.FindOneAndUpdateAsync(filter, update);
            if (result == null){
                throw new InvalidPlayerIDException("PlayerID "+id+" not found");
            }
            return await Get(id);
        }

        public async Task<Player> Delete(Guid id){
            Player player = Get(id).Result;
            await Collection.DeleteOneAsync(Builders<Player>.Filter.Eq("_id",id));
            if (player != null){
                return player;
            } else {
                throw new InvalidPlayerIDException(id.ToString());
            }
        }
        public async Task<bool> DeleteAll(){
            await Collection.DeleteManyAsync(_=>true);
            return true;
        }


        public async Task<Item> PushItem(Guid id, Item item){
            FilterDefinition<Player> filter = Builders<Player>.Filter.Eq("_id",id);
            var result = await Collection.UpdateOneAsync(filter, Builders<Player>.Update.Push("Items", item));
            if (result.IsAcknowledged){
                return item;
            } else {
                return (Item)null;
            }
            
        }

        public async Task<String> GetCommonLevelBetweenPlayers(){
            var pipeline = new BsonDocument[]{
                new BsonDocument{{"$project" ,new BsonDocument("Level" , 1)}},
                new BsonDocument{{"$group" , new BsonDocument {{"_id" , "$Level"},{"Count" , new BsonDocument ("$sum" , 1 )}}}},
                new BsonDocument{{"$sort", new BsonDocument("Count", -1)}},
                new BsonDocument{{"$limit", 3}}
            };
            var result = await Collection.AggregateAsync<BsonDocument>(pipeline);
            BsonValue value;
            BsonValue count;
            var resList = result.ToList();
            resList[0].TryGetValue("Count", out count);
            resList[0].TryGetValue("_id", out value);
            return "Most common level is "+value+" with count of "+count;
        }

        public async Task<String> GetAverageScorePerDates(DateTime startTime, DateTime endTime){
            var pipe2 = Collection.Aggregate().
                Project(r => new { Creation = r.CreationTime, Score = r.Score}).
                Match(r=> r.Creation >=startTime && r.Creation <= endTime).
                Group(r=>1,g=> new {ScoreG = g.Average(r=>r.Score)});
            Console.WriteLine(pipe2.ToList().Count);
            double ret = (await pipe2.FirstAsync()).ScoreG;
            return "Average score for players created between "+startTime+" and "+endTime+" is "+ret;
        }





        public async Task WriteLog(string str){
            Log log = new Log();
            log.id = Guid.NewGuid();
            log.logStr = str;
            await LogCollection.InsertOneAsync(log);
        }

        public async Task<string> GetLogs(){
            var result = LogCollection.Find(_=>true);
            var resList = await result.ToListAsync();
            string retList = "";
            foreach(Log log in resList){
                retList += log.logStr+Environment.NewLine;
            }
            return retList;

        }
    }
}