using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Factories.ShoppingLists;

/// <summary>
/// Defines the contract for a factory that creates instances of <see cref="ShoppingListItem"/>.
/// </summary>
public interface IShoppingListItemFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ShoppingListItem"/> for the specified shopping list.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="request">The request containing the details of the shopping list item to create.</param>
    /// <returns>A new instance of <see cref="ShoppingListItem"/>.</returns>
    ShoppingListItem CreateShoppingListItem(Guid shoppingListId, CreateShoppingListItemRequest request);
}
