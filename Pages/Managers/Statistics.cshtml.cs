using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Pages.Managers
{
    [Authorize(Roles = "managers")]
    public class StatisticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StatisticsModel(ApplicationDbContext context)
        {
            _context = context;
            TravellerStatistics = new List<TravellerStatistic>();
            BookingStatistics = new List<BookingStatistic>();
        }

        public List<TravellerStatistic> TravellerStatistics { get; set; }
        public List<BookingStatistic> BookingStatistics { get; set; }

        public void OnGet()
        {
            // Calculate statistics for traveller distribution
            TravellerStatistics = _context.Traveller
                .GroupBy(t => t.Postcode)
                .Select(group => new TravellerStatistic
                {
                    Postcode = group.Key,
                    NumberOfTravellers = group.Count()
                })
                .OrderByDescending(stat => stat.NumberOfTravellers)
                .ToList();

            // Calculate statistics for booking distribution
            BookingStatistics = _context.Booking
                .GroupBy(b => b.ApartmentID)
                .Select(group => new BookingStatistic
                {
                    ApartmentID = group.Key,
                    NumberOfBookings = group.Count()
                })
                .OrderByDescending(stat => stat.NumberOfBookings)
                .ToList();
        }

    }
}
