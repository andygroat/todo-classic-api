using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Classic.Model.Domain.ShoppingLists;

/// <summary>
/// Represents a shopping list in the application.
/// </summary>
[Table("ShoppingLists", Schema = Constants.Schemas.Default)]
public sealed class ShoppingList : BusinessObject
{
    /// <summary>
    /// Gets or sets the title of the shopping list.
    /// </summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the items that belong to this shopping list.
    /// </summary>
    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}
