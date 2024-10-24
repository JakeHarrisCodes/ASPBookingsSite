using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Pages.Bookings
{
    [Authorize(Roles = "managers")]
    public class EditModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public EditModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.Include(b => b.TheRoom).Include(b => b.TheTraveller).FirstOrDefaultAsync(m => m.ID == id);

            if (booking == null)
            {
                return NotFound();
            }

            Booking = booking;

            // Generate the dropdown list for Apartment IDs
            ViewData["ApartmentID"] = new SelectList(_context.Apartment, "ID", "ID", booking.TheRoom.ID);

            // Generate the dropdown list for Traveller emails
            ViewData["TravellerEmail"] = new SelectList(_context.Traveller, "Email", "FullName", booking.TheTraveller?.Email);

            return Page();
        }

        public string ErrorMessage { get; set; }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if CheckIn is less than today's date
            if (Booking.CheckIn.Date < DateTime.Today)
            {
                ModelState.AddModelError($"{nameof(Booking)}.{nameof(Booking.CheckIn)}", "Check In date cannot be less than today's date.");
                PopulateDropdowns(Booking.ID);
                return Page();
            }

            // Check if CheckOut is less than or equal to CheckIn
            if (Booking.CheckOut.Date <= Booking.CheckIn.Date)
            {
                ModelState.AddModelError($"{nameof(Booking)}.{nameof(Booking.CheckOut)}", "Check Out date must be later than Check-In date.");
                PopulateDropdowns(Booking.ID);
                return Page();
            }

            // Check availability using raw SQL
            string checkAvailabilitySql = @"
                SELECT COUNT(*) 
                FROM Booking 
                WHERE ApartmentID = @ApartmentID 
                    AND NOT (
                        CheckOut <= @CheckIn 
                        OR CheckIn >= @CheckOut
                    )";

            var overlappingBookingsCount = await _context.Booking
                .Where(b => b.ApartmentID == Booking.ApartmentID &&
                            !(b.CheckOut <= Booking.CheckIn || b.CheckIn >= Booking.CheckOut) &&
                            (b.ID != Booking.ID || Booking.ID == 0))  // Check for Booking ID
                .CountAsync();


            if (overlappingBookingsCount > 0)
            {
                // Apartment is not available
                ErrorMessage = "This apartment is not available for the selected dates.";
                PopulateDropdowns(Booking.ID); // Repopulate dropdowns
                return Page();
            }

            _context.Attach(Booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(Booking.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Set success message
            TempData["SuccessMsg"] = "Booking edited successfully.";

            return RedirectToPage("./Index");
        }

        private bool BookingExists(int id)
        {
            return (_context.Booking?.Any(e => e.ID == id)).GetValueOrDefault();
        }

        // Because on a form error like apartment isn't available, then the dropdowns go blank!
        private void PopulateDropdowns(int bookingId)
        {
            var booking = _context.Booking
                .Include(b => b.TheRoom)
                .Include(b => b.TheTraveller)
                .FirstOrDefault(m => m.ID == bookingId);

            // Generate the dropdown list for Apartment IDs
            ViewData["ApartmentID"] = new SelectList(_context.Apartment, "ID", "ID", booking?.TheRoom?.ID);

            // Generate the dropdown list for Traveller emails
            ViewData["TravellerEmail"] = new SelectList(_context.Traveller, "Email", "FullName", booking?.TheTraveller?.Email);
        }
    }
}
