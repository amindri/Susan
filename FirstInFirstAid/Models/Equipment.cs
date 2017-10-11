using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FirstInFirstAid.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; }

        public ICollection<EquipmentAllocation> EquipmentAllocations { get; set; }
    }
}