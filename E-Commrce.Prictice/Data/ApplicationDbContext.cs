using E_Commrce.Prictice.Model;
using Microsoft.EntityFrameworkCore;

namespace E_Commrce.Prictice.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options)
        {
            
        }

        public DbSet<OtpToken> OtpTokens { get; set; }  
      public DbSet<User>Users { get; set; }

    }
}
