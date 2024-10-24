using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class Apartment
    {
        public int ID { get; set; }

        [Required]
        [RegularExpression("^[G123]$", ErrorMessage = "The apartment level must be 1 character from: G, 1, 2, or 3.")]
        public string? Level { get; set; }

        [Required]
        [Display(Name = "Bedroom Count")]
        [Range(1, 3, ErrorMessage = "Bedroom count can only be: 1, 2, or 3.")]
        public int BedroomCount { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(200, 500)]
        public decimal Price { get; set; }

        // Navigation property
        [Display(Name = "The Bookings")]
        public ICollection<Booking>? TheBookings { get; set; }
    }
}
