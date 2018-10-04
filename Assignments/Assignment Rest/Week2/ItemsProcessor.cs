using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Week2
{
    public class ItemsProcessor
    {
        IRepository repository;
        public ItemsProcessor(IRepository rep){
            repository = rep;
        }

        public async Task<List<Item>> GetAllItems(Guid playerId){
            Player player = await repository.Get(playerId);
            return player.Inventory;
        }

        public async Task<Item> GetAnItem(Guid playerId, int itemPos){
            Player player = await repository.Get(playerId);
            try{
                return player.Inventory[itemPos];
            } catch{
                return null;
            }
        }

        public async Task<Item> AddItem(Guid playerId, NewItem newItem){
            Player player = await repository.Get(playerId);
            if (player != null){
                Item item = new Item();
                item.Level = newItem.Level;
                item.Type = newItem.Type;
                item.CreationDate = newItem.CreationDate;
                ItemCheck(item.Type, player.Level);
                /*
                player.Inventory.Add(item);
                Player playerNew = await repository.Replace(player);
                if (playerNew != null){
                    return item;
                }
                */
                return await repository.PushItem(playerId, item);
            } 
            return (Item)null;
        }

        public async Task<Item> ModifyItem(Guid playerId, int itemPos, ModifiedItem modItem){
            Player player = await repository.Get(playerId);
            if(player.Inventory.Count > itemPos && itemPos >= 0){
                Item item = player.Inventory[itemPos];
                if(item != null){
                    ItemCheck(item.Type, player.Level);
                    item.Level = modItem.Level;
                    Player player2 = await repository.Replace(player);
                    if (player2 != null){
                        return item;
                    }
                }
            }
            return null;
        }

        public async Task<bool> DeleteItem(Guid playerId, int itemPos){
            Player player = await repository.Get(playerId);
            if(player.Inventory.Count > itemPos && itemPos >= 0){
                if (player.Inventory[itemPos] != null){
                    player.Inventory.RemoveAt(itemPos);
                    Player player2 = await repository.Replace(player);
                    if (player2 != null){
                        return true;
                    }
                }
            }
            return false;
        }


        // Game rule 1
        public bool ItemCheck(string itemType, int playerLevel){
            if(itemType == validItemType.sword.ToString() && playerLevel < 3){
                throw new InvalidItemException("Swords for players below level 3 are not allowed");
            }
            return true;
        }
    }
}