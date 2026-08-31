using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Services.ShoppingLists;

/// <summary>
/// Defines the contract for a service that manages shopping lists.
/// </summary>
public interface IShoppingListService
{
    /// <summary>
    /// Creates a new shopping list.
    /// </summary>
    /// <param name="request">The request containing the details of the shopping list to create.</param>
    /// <returns>The unique identifier of the created shopping list.</returns>
    Task<Guid> CreateShoppingListAsync(CreateShoppingListRequest request);

    /// <summary>
    /// Gets shopping lists, optionally filtered by a search string matching the title.
    /// </summary>
    /// <param name="search">An optional search string to filter shopping lists by title.</param>
    /// <returns>A collection of shopping lists matching the search criteria.</returns>
    Task<IReadOnlyList<ShoppingListDto>> GetShoppingListsAsync(string? search = null);

    /// <summary>
    /// Gets a shopping list by its unique identifier, including its items.
    /// </summary>
    /// <param name="id">The unique identifier of the shopping list.</param>
    /// <returns>The matching shopping list, or <c>null</c> if not found.</returns>
    Task<ShoppingListDto?> GetShoppingListByIdAsync(Guid id);

    /// <summary>
    /// Updates the title of an existing shopping list.
    /// </summary>
    /// <param name="id">The unique identifier of the shopping list to update.</param>
    /// <param name="request">The updated shopping list details.</param>
    /// <returns>The updated shopping list, or <c>null</c> if not found.</returns>
    Task<ShoppingListDto?> UpdateShoppingListAsync(Guid id, UpdateShoppingListRequest request);

    /// <summary>
    /// Deletes a shopping list along with all of its items.
    /// </summary>
    /// <param name="id">The unique identifier of the shopping list to delete.</param>
    /// <returns><c>true</c> if the shopping list was deleted; otherwise, <c>false</c>.</returns>
    Task<bool> DeleteShoppingListAsync(Guid id);
}
