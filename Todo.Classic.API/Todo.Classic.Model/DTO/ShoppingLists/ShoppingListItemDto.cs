namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a shopping list item returned to clients.
/// </summary>
/// <param name="Id">The unique identifier of the shopping list item.</param>
/// <param name="ShoppingListId">The identifier of the parent shopping list.</param>
/// <param name="Title">The title of the shopping list item.</param>
/// <param name="IsComplete">Whether the shopping list item is complete.</param>
public record ShoppingListItemDto(Guid Id, Guid ShoppingListId, string Title, bool IsComplete);
