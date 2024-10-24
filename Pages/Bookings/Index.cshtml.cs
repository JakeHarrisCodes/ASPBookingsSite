using System;
using System.Collections.Generic;
using System.Linq;
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
    public class IndexModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public IndexModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Booking> Booking { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Booking != null)
            {
                Booking = await _context.Booking
                .Include(b => b.TheRoom)
                .Include(b => b.TheTraveller).ToListAsync();
            }
        }
    }
}
