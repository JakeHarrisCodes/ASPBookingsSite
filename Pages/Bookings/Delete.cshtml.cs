using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Pages.Bookings
{
    [Authorize(Roles = "managers")]
    public class DeleteModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public DeleteModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
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

            var booking = await _context.Booking
                .Include(b => b.TheRoom)
                .Include(b => b.TheTraveller)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (booking == null)
            {
                return NotFound();
            }
            else
            {
                Booking = booking;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Booking == null)
            {
                return NotFound();
            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking != null)
            {
                _context.Booking.Remove(booking);
                await _context.SaveChangesAsync();

                // Set success message
                TempData["SuccessMsg"] = "Booking deleted successfully.";
            }

            return RedirectToPage("./Index");
        }
    }
}
