using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace PSA_JH_YR_AB.Pages.Bookings
{
    [Authorize(Roles = "travellers")]
    public class SearchApartmentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public SearchApartments SearchModel { get; set; }
        public IActionResult OnGet()
        {
            // So we can hide the search results by default
            ViewData["ActiveSearch"] = "false";
            return Page();
        }

        // create a list so we can pass it to the frontend page
        public List<Apartment> Result { get; set; }

        public SearchApartmentsModel(ApplicationDbContext context)
        {
            _context = context;
            Result = new List<Apartment>(); // Initialise here otherwise we get some null error
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if CheckIn is less than today's date
            if (SearchModel.CheckIn?.Date < DateTime.Today)
            {
                ModelState.AddModelError($"{nameof(SearchModel)}.{nameof(SearchModel.CheckIn)}", "Check In date cannot be less than today's date.");
                return Page();
            }

            // Check if both CheckIn and CheckOut have values, and CheckOut is less than or equal to CheckIn
            if (SearchModel.CheckIn.HasValue && SearchModel.CheckOut.HasValue && SearchModel.CheckOut.Value.Date <= SearchModel.CheckIn.Value.Date)
            {
                ModelState.AddModelError($"{nameof(SearchModel)}.{nameof(SearchModel.CheckOut)}", "Check Out date must be later than Check-In date.");
                return Page();
            }

            ViewData["ActiveSearch"] = "true";

            // Constructing raw SQL query
            var query = $"SELECT * FROM Apartment WHERE BedroomCount = {SearchModel.NumberOfBedrooms} AND Id NOT IN (" +
                        $"SELECT DISTINCT ApartmentId FROM Booking " +
                        $"WHERE (CheckIn < '{SearchModel.CheckOut:yyyy-MM-dd}' AND CheckOut > '{SearchModel.CheckIn:yyyy-MM-dd}'))";

            // Executing the query
            Result = await _context.Apartment.FromSqlRaw(query).ToListAsync();

            return Page();
        }

    }
}
