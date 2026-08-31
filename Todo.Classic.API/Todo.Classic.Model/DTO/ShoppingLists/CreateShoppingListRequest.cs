namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a request to create a new shopping list.
/// </summary>
/// <param name="Title">The title of the shopping list.</param>
public record CreateShoppingListRequest(string Title);
