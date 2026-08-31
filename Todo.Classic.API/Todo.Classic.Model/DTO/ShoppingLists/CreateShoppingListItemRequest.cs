namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a request to create a new shopping list item.
/// </summary>
/// <param name="Title">The title of the shopping list item.</param>
public record CreateShoppingListItemRequest(string Title);
