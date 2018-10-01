using System.ComponentModel.DataAnnotations;

namespace Week2
{
    public class ModifiedPlayer
    {
        public int? Score { get; set; }
        [Range(1,99)]
        public int? Level { get; set; }
        public PlayerTag[] Tags{ get ; set; }
    }
}