using System;
using System.ComponentModel.DataAnnotations;

namespace Week2
{

    public interface IItem{
        int GetLevel();
        string GetItemType();
        DateTime GetCreationDate();
    }
    public class Item
    {
        public int Level { get; set; }
        public string Type{ get; set; }
        public DateTime CreationDate{ get; set; }
    }

    public enum validItemType {sword, notSword};

    public class ItemTypeValidation : ValidationAttribute{
        protected override ValidationResult IsValid(object value, ValidationContext validationContext){
            IItem item = (IItem)validationContext.ObjectInstance;
            foreach(validItemType itemType in Enum.GetValues(typeof(validItemType))){
                if (itemType.ToString() == item.GetItemType()){
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Item type not valid"); 
        }
    }

    public class ItemDateValidation : ValidationAttribute{
        protected override ValidationResult IsValid(object value, ValidationContext validationContext){
            IItem item = (IItem)validationContext.ObjectInstance;
            if (item.GetCreationDate() <= DateTime.UtcNow){
                return ValidationResult.Success;
            }
            return new ValidationResult("Item creation date not valid");
        }
    }
}