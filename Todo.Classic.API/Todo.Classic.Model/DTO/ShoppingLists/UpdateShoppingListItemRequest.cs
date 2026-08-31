namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a request to update an existing shopping list item.
/// </summary>
/// <param name="Title">The new title of the shopping list item.</param>
public record UpdateShoppingListItemRequest(string Title);
