using System.ComponentModel.DataAnnotations;

namespace Week2
{
    public class ModifiedItem
    {
        [Range(1,99)]
        public int Level;
    }
}