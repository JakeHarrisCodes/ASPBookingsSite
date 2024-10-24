using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class TravellerStatistic
    {
        public string? Postcode { get; set; }

        [Display(Name = "Number of Travellers")]
        public int NumberOfTravellers { get; set; }
    }
}
