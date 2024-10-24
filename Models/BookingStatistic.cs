using System.ComponentModel.DataAnnotations;

namespace PSA_JH_YR_AB.Models
{
    public class BookingStatistic
    {
        [Display(Name = "Apartment ID")]
        public int ApartmentID { get; set; }

        [Display(Name = "Number of Bookings")]
        public int NumberOfBookings { get; set; }
    }
}
