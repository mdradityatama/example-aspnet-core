using ExampleUploadFile.WebUI.Models;
using Microsoft.EntityFrameworkCore;

namespace ExampleUploadFile.WebUI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<AppFile> Files { get; set; }
    }
}