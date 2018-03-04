using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace FirstInFirstAid.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Equipment name is missing")]
        public string EquipmentName { get; set; }

        public ICollection<EquipmentAllocation> EquipmentAllocations { get; set; }
    }
}