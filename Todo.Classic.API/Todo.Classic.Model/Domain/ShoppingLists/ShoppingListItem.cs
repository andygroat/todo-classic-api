using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.Classic.Model.Domain.ShoppingLists;

/// <summary>
/// Represents an item on a shopping list.
/// </summary>
[Table("ShoppingListItems", Schema = Constants.Schemas.Default)]
public sealed class ShoppingListItem : BusinessObject
{
    /// <summary>
    /// Gets or sets the title of the shopping list item.
    /// </summary>
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the item has been completed (e.g. purchased).
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the parent shopping list.
    /// </summary>
    [Required]
    public Guid ShoppingListId { get; set; }

    /// <summary>
    /// Gets or sets the parent shopping list navigation property.
    /// </summary>
    [ForeignKey(nameof(ShoppingListId))]
    public ShoppingList? ShoppingList { get; set; }
}
