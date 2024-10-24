using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PSA_JH_YR_AB.Pages.Bookings
{
    [Authorize(Roles = "travellers")]
    public class BookApartmentModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty(SupportsGet = true)] 
        [Display(Name = "Apartment ID")]
        public int ApartmentId { get; set; }

        [BindProperty(SupportsGet = true)] 
        [Display(Name = "Check In Date")]
        public DateTime CheckIn { get; set; }

        [BindProperty(SupportsGet = true)] 
        [Display(Name = "Check Out Date")]
        public DateTime CheckOut { get; set; }

        public string ErrorMessage { get; set; }

        public List<Apartment> AvailableApartments { get; set; }

        public BookApartmentModel(ApplicationDbContext context)
        {
            _context = context;
            AvailableApartments = new List<Apartment>(); // Initialise here otherwise we get some null error
        }

        public async Task OnGetAsync()
        {            
            AvailableApartments = await _context.Apartment.ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                AvailableApartments = await _context.Apartment.ToListAsync();
                return Page();
            }

            // Check if CheckIn is less than today's date
            if (CheckIn.Date < DateTime.Today)
            {
                ModelState.AddModelError(nameof(CheckIn), "Check In date cannot be less than today's date.");
                AvailableApartments = await _context.Apartment.ToListAsync();
                return Page();
            }

            // Check if CheckOut is less than or equal to CheckIn
            if (CheckOut.Date <= CheckIn.Date)
            {
                ModelState.AddModelError(nameof(CheckOut), "Check Out date must be later than Check-In date.");
                AvailableApartments = await _context.Apartment.ToListAsync();
                return Page();
            }

            var apartment = await _context.Apartment.FindAsync(ApartmentId);

            if (apartment == null)
            {
                // Handle the case where the specified apartment is not found
                return NotFound();
            }


                // Validate availability using raw SQL query
                var availableApartments = await _context.Apartment
                .FromSqlRaw(GetAvailabilityQuery(), ApartmentId, CheckIn, CheckOut)
                .ToListAsync();

                if (availableApartments.Any())
                {
                    int numberOfNights = (int)(CheckOut - CheckIn).TotalDays;

                    // Calculate the cost
                    decimal cost = apartment.Price * numberOfNights;

                    var booking = new Booking
                    {
                        ApartmentID = ApartmentId,
                        TravellerEmail = User.Identity?.Name,
                        CheckIn = CheckIn,
                        CheckOut = CheckOut,
                        Cost = cost
                    };

                    _context.Booking.Add(booking);
                    await _context.SaveChangesAsync();

                    var apartmentLevel = await _context.Apartment
                        .Where(a => a.ID == ApartmentId)
                        .Select(a => a.Level)
                        .FirstOrDefaultAsync();

                    ViewData["SuccessBooking"] = "success";
                    ViewData["ApartmentID"] = ApartmentId;
                    ViewData["ApartmentLevel"] = apartmentLevel;
                    ViewData["CheckIn"] = CheckIn;
                    ViewData["CheckOut"] = CheckOut;
                    ViewData["TotalPrice"] = cost;

                    AvailableApartments = await _context.Apartment.ToListAsync();

                    return Page();
                }
                else
                {
                    ErrorMessage = "This apartment is not available for the selected dates.";
                    AvailableApartments = await _context.Apartment.ToListAsync();
                    return Page();
                }
        }


        private string GetAvailabilityQuery()
        {
            return @"
                SELECT DISTINCT a.*
                FROM Apartment a
                WHERE a.ID = {0}
                AND NOT EXISTS (
                    SELECT 1
                    FROM Booking b
                    WHERE b.ApartmentID = a.ID
                    AND NOT (
                        b.CheckOut <= {1} OR
                        {2} <= b.CheckIn
                    )
                )";
        }
    }
}
