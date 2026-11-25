global using Microsoft.EntityFrameworkCore;

namespace PickNest.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions option):base(option)
        {

        }
        public DbSet<Products> Product { get; set; }
    }
}
