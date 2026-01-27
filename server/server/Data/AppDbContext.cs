using Microsoft.EntityFrameworkCore;
using server.Domain;
using Server.Domain;


namespace server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserRecipe> UserRecipes { get; set; }
    public DbSet<UserRecipeIngredients> UserRecipeIngredients { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follow> Follows { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Recipe>()
            .HasOne(r => r.Photos)
            .WithOne(r => r.Recipe)
            .HasForeignKey<Photo>(p => p.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRecipe>()
        .HasMany(r => r.Ingredients)
        .WithOne(i => i.Recipe)
        .HasForeignKey(i => i.RecipeId)
        .OnDelete(DeleteBehavior.Cascade);

        // Configure Follow relationships - User can have many followers and follow many users
        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
