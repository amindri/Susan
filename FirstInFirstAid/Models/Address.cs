using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum State { SA, NT, WA, QLD, ACT, NSW, VIC, TAS }
    public class Address
    {     
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Street Number missing")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Street Name missing")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Suburb missing")]
        public string Suburb { get; set; }

        [Required(ErrorMessage = "Postcode missing")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "State missing")]
        public State? State { get; set; }
    }
}