﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Data;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Pages.Apartments
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public DeleteModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Apartment Apartment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Apartment == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FirstOrDefaultAsync(m => m.ID == id);

            if (apartment == null)
            {
                return NotFound();
            }
            else 
            {
                Apartment = apartment;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Apartment == null)
            {
                return NotFound();
            }
            var apartment = await _context.Apartment.FindAsync(id);

            if (apartment != null)
            {
                Apartment = apartment;
                _context.Apartment.Remove(Apartment);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
