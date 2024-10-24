using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class Booking
    {
        // PK
        public int ID { get; set; }

        // FK
        public int ApartmentID { get; set; }

        // FK Email
        [DataType(DataType.EmailAddress)]
        public string? TravellerEmail { get; set; }

        [Required]
        [Display(Name = "Check In")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckIn { get; set; }

        [Required]
        [Display(Name = "Check Out")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckOut { get; set; }

        [Required]
        [Range(0.0, 10000.0)]
        public decimal Cost { get; set; }

        // Navigation Properties
        [Display(Name = "The Room")]
        public Apartment? TheRoom { get; set; }

        [Display(Name = "The Traveller")]
        public Traveller? TheTraveller { get; set; }
    }
}
