using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class Traveller
    {
        [Key]
        [Required]
        [DataType(DataType.EmailAddress)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z'-]+$", ErrorMessage = "Surname can only consist of English letters, hyphen and apostrophe.")]
        [StringLength(20, MinimumLength = 2)]
        public string? Surname { get; set; }

        [Required]
        [Display(Name = "Given Name")]
        [RegularExpression(@"^[A-Za-z'-]+$", ErrorMessage = "Given Name can only consist of English letters, hyphen and apostrophe.")]
        [StringLength(20, MinimumLength = 2)]
        public string? GivenName { get; set; }

        // For Part 7.1.2 -> 2. in the project specification !!
        [NotMapped]
        public string FullName => $"{GivenName} {Surname}";

        [Required]
        [RegularExpression(@"[0-9]{4}$", ErrorMessage = "Post Code can only contain numbers and must be 4 digits. Example: 2100")]
        public string? Postcode { get; set; }

        // Navigation property
        [Display(Name = "The Bookings")]
        public ICollection<Booking>? TheBookings { get; set; }
    }
}