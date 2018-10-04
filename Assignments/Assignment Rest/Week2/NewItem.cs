using System;
using System.ComponentModel.DataAnnotations;

namespace Week2
{
    public class NewItem : IItem
    {
        [Range(1,99)]
        public int Level{ get; set; }
        [ItemTypeValidation]
        public string Type{ get; set; }
        [ItemDateValidation]
        public DateTime CreationDate{ get; set; }

        public int GetLevel(){
            return Level;
        }
        public string GetItemType(){
            return Type;
        }

        public DateTime GetCreationDate(){
            return CreationDate;
        }
    }
}