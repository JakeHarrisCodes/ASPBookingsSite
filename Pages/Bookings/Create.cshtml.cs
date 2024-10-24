using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;
using Microsoft.Data.Sqlite;

namespace PSA_JH_YR_AB.Pages.Bookings
{
    [Authorize(Roles = "managers")]
    public class CreateModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public CreateModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public SelectList TravellerList { get; set; } = default!;

        public IActionResult OnGet()
        {
            PopulateTravellerDropDownList();
            return Page();
        }

        public string ErrorMessage { get; set; }

        [BindProperty]
        public Booking Booking { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Booking == null || Booking == null)
            {
                PopulateTravellerDropDownList();
                return Page();
            }

            // Check if CheckIn is less than today's date
            if (Booking.CheckIn.Date < DateTime.Today)
            {
                ModelState.AddModelError($"{nameof(Booking)}.{nameof(Booking.CheckIn)}", "Check In date cannot be less than today's date.");
                PopulateTravellerDropDownList();
                return Page();
            }

            // Check if CheckOut is less than or equal to CheckIn
            if (Booking.CheckOut.Date <= Booking.CheckIn.Date)
            {
                ModelState.AddModelError($"{nameof(Booking)}.{nameof(Booking.CheckOut)}", "Check Out date must be later than Check-In date.");
                PopulateTravellerDropDownList();
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
                            !(b.CheckOut <= Booking.CheckIn || b.CheckIn >= Booking.CheckOut))
                .CountAsync();

            if (overlappingBookingsCount > 0)
            {
                // Apartment is not available
                ErrorMessage = "This apartment is not available for the selected dates.";
                PopulateTravellerDropDownList();
                return Page();
            }

            _context.Booking.Add(Booking);
            await _context.SaveChangesAsync();

            // Set success message - use tempdata otherwise it will be lost in the redirect!!!
            TempData["SuccessMsg"] = "Booking created successfully.";

            return RedirectToPage("./Index");
        }

        // Our custom method to put data in the traveller dropdown!
        private void PopulateTravellerDropDownList()
        {
            var travellers = _context.Traveller.Select(t => new { t.Email, t.FullName }).ToList();
            TravellerList = new SelectList(travellers, "Email", "FullName");
        }
    }
}
