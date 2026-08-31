namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a request to update an existing shopping list.
/// </summary>
/// <param name="Title">The new title of the shopping list.</param>
public record UpdateShoppingListRequest(string Title);
