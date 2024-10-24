using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Pages.Travellers
{
    [Authorize(Roles = "travellers")]
    public class MyDetailsModel : PageModel
    {
        private readonly PSA_JH_YR_AB.Data.ApplicationDbContext _context;

        public MyDetailsModel(PSA_JH_YR_AB.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Traveller? Myself { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // retrieve the logged-in user's email
            // need to add "using System.Security.Claims;"
            string _email = User.FindFirst(ClaimTypes.Name).Value;

            Traveller traveller = await _context.Traveller.FirstOrDefaultAsync(m => m.Email == _email);

            /* dealing with both cases of new user and existing user */
            if (traveller != null)
            {
                // existing user
                ViewData["ExistInDB"] = "true";
                Myself = traveller;
            }
            else
            {
                // new user
                ViewData["ExistInDB"] = "false";
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            // retrieve the logged-in user's email
            // need to add "using System.Security.Claims;"
            string _email = User.FindFirst(ClaimTypes.Name).Value;

            Traveller traveller = await _context.Traveller.FirstOrDefaultAsync(m => m.Email == _email);

            /* dealing with both cases of new user and existing user */
            if (traveller != null)
            {
                // This ViewData entry is needed in the content file
                // The user has been created in the database
                ViewData["ExistInDB"] = "true";
            }
            else
            {
                // new user
                ViewData["ExistInDB"] = "false";
                traveller = new Traveller();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            /* for preventing overposting attacks */
            traveller.Email = _email;

            var success = await TryUpdateModelAsync<Traveller>(traveller, "Myself",
                                t => t.GivenName, t => t.Surname, t => t.Postcode);
            if (!success)
            {
                return Page();
            }

            if ((string)ViewData["ExistInDB"] == "true")
            {
                // Since the context doesn't allow tracking two objects with the same key,
                // we do the copying first, and then update.
                _context.Traveller.Update(traveller);
            }
            else
            {
                // add this newly-created record into db
                _context.Traveller.Add(traveller);
            }

            try  // catching the conflict of editing this record concurrently
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            ViewData["SuccessDB"] = "success";
            return Page();
        }
    }
}
