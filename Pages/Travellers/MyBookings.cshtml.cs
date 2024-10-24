using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSA_JH_YR_AB.Pages.Travellers
{
    [Authorize(Roles = "travellers")]
    public class MyBookingsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyBookingsModel(ApplicationDbContext context)
        {
            _context = context;
            Bookings = new List<Booking>();
        }

        public IList<Booking> Bookings { get; set; }

        public async Task OnGetAsync(string sortOrder = "checkin_asc")
        {
            // Get the logged in user's email
            var userEmail = User.Identity?.Name;

            // Fetch the bookings for the logged in user
            IQueryable<Booking> bookingsQuery = _context.Booking
                .Include(b => b.TheTraveller)
                .Where(b => b.TravellerEmail == userEmail);

            var bookings = await bookingsQuery.ToListAsync();

            // Sorting
            Bookings = sortOrder switch
            {
                "checkin_desc" => bookings.OrderByDescending(b => b.CheckIn).ToList(),
                "cost_asc" => bookings.OrderBy(b => b.Cost).ToList(),
                "cost_desc" => bookings.OrderByDescending(b => b.Cost).ToList(),
                _ => bookings.OrderBy(b => b.CheckIn).ToList(), // Default sorting (check in date asc)
            };

            ViewData["NextCheckinOrder"] = sortOrder != "checkin_asc" ? "checkin_asc" : "checkin_desc";
            ViewData["NextCostOrder"] = sortOrder != "cost_asc" ? "cost_asc" : "cost_desc";

            // Bookings count to the frontend
            ViewData["TotalBookingsCount"] = bookings.Count;
        }

    }
}
