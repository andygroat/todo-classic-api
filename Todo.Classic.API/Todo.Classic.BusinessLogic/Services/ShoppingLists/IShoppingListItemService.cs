using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Services.ShoppingLists;

/// <summary>
/// Defines the contract for a service that manages shopping list items.
/// </summary>
public interface IShoppingListItemService
{
    /// <summary>
    /// Creates a new shopping list item on the specified shopping list.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="request">The request containing the details of the item to create.</param>
    /// <returns>The identifier of the created item, or <c>null</c> if the parent shopping list was not found.</returns>
    Task<Guid?> CreateShoppingListItemAsync(Guid shoppingListId, CreateShoppingListItemRequest request);

    /// <summary>
    /// Gets the items for the specified shopping list.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <returns>The collection of items, or <c>null</c> if the parent shopping list was not found.</returns>
    Task<IReadOnlyList<ShoppingListItemDto>?> GetShoppingListItemsAsync(Guid shoppingListId);

    /// <summary>
    /// Gets a shopping list item by its unique identifier.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="itemId">The unique identifier of the item.</param>
    /// <returns>The matching item, or <c>null</c> if not found.</returns>
    Task<ShoppingListItemDto?> GetShoppingListItemByIdAsync(Guid shoppingListId, Guid itemId);

    /// <summary>
    /// Updates the title of an existing shopping list item.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="itemId">The identifier of the item to update.</param>
    /// <param name="request">The updated item details.</param>
    /// <returns>The updated item, or <c>null</c> if not found.</returns>
    Task<ShoppingListItemDto?> UpdateShoppingListItemAsync(Guid shoppingListId, Guid itemId, UpdateShoppingListItemRequest request);

    /// <summary>
    /// Marks a shopping list item as complete.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="itemId">The identifier of the item to complete.</param>
    /// <returns>The updated item, or <c>null</c> if not found.</returns>
    Task<ShoppingListItemDto?> CompleteShoppingListItemAsync(Guid shoppingListId, Guid itemId);

    /// <summary>
    /// Deletes a shopping list item.
    /// </summary>
    /// <param name="shoppingListId">The identifier of the parent shopping list.</param>
    /// <param name="itemId">The identifier of the item to delete.</param>
    /// <returns><c>true</c> if the item was deleted; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteShoppingListItemAsync(Guid shoppingListId, Guid itemId);
}
