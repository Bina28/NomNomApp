using Microsoft.EntityFrameworkCore;
using server.Domain;


namespace server.Data;

public class AppDbContext: DbContext 
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }   
    public DbSet<Photo> Photos { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Recipe>()
            .HasOne( r => r.Photos)
            .WithOne( r => r.Recipe )
            .HasForeignKey<Photo>(p => p.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }

}
