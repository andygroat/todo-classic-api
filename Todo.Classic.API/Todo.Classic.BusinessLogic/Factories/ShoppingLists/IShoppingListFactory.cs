using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Factories.ShoppingLists;

/// <summary>
/// Defines the contract for a factory that creates instances of <see cref="ShoppingList"/>.
/// </summary>
public interface IShoppingListFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="ShoppingList"/> based on the provided <see cref="CreateShoppingListRequest"/>.
    /// </summary>
    /// <param name="request">The request containing the details of the shopping list to create.</param>
    /// <returns>A new instance of <see cref="ShoppingList"/>.</returns>
    ShoppingList CreateShoppingList(CreateShoppingListRequest request);
}
