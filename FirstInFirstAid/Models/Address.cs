using System.ComponentModel.DataAnnotations;

namespace FirstInFirstAid.Models
{
    public enum State { SA, NT, WA, QLD, ACT, NSW, VIC, TAS }
    public class Address
    {     
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Street number is missing")]
        public string StreetNumber { get; set; }

        [Required(ErrorMessage = "Street name is missing")]
        public string StreetName { get; set; }

        [Required(ErrorMessage = "Suburb is missing")]
        public string Suburb { get; set; }

        [Required(ErrorMessage = "Postcode is missing")]
        [RegularExpression("(\\d{4})", ErrorMessage = "Please enter a valid Postcode")]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "State is missing")]
        public State? State { get; set; }
    }
}