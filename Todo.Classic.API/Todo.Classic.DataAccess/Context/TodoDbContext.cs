using Microsoft.EntityFrameworkCore;
using Todo.Classic.Model.Constants;
using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.Domain.Todos;

namespace Todo.Classic.DataAccess.Context;

public sealed class TodoDbContext (DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet of TodoItem entities.
    /// </summary>
    public DbSet<TodoItem> TodoItems { get; set; }

    /// <summary>
    /// Gets or sets the DbSet of ShoppingList entities.
    /// </summary>
    public DbSet<ShoppingList> ShoppingLists { get; set; }

    /// <summary>
    /// Gets or sets the DbSet of ShoppingListItem entities.
    /// </summary>
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Default);

        modelBuilder.Entity<ShoppingList>()
            .HasMany(l => l.Items)
            .WithOne(i => i.ShoppingList!)
            .HasForeignKey(i => i.ShoppingListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
