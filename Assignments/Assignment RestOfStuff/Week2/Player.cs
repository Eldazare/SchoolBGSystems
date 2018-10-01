using System;
using System.Collections.Generic;

namespace Week2
{

    public enum PlayerTag{None, Good, Bad, Fubar};

    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Score { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreationTime { get; set; }
        public List<Item> Inventory{ get; set; }
        public List<PlayerTag> TagList{ get; set; }
    }
}