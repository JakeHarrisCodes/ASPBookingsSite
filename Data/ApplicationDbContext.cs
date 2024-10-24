using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSA_JH_YR_AB.Models;

namespace PSA_JH_YR_AB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PSA_JH_YR_AB.Models.Apartment> Apartment { get; set; } = default!;
        public DbSet<PSA_JH_YR_AB.Models.Booking> Booking { get; set; } = default!;
        public DbSet<PSA_JH_YR_AB.Models.Traveller> Traveller { get; set; } = default!;

    }
}