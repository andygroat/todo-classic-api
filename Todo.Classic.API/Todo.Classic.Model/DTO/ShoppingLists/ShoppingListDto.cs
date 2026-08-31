namespace Todo.Classic.Model.DTO.ShoppingLists;

/// <summary>
/// Represents a shopping list returned to clients.
/// </summary>
/// <param name="Id">The unique identifier of the shopping list.</param>
/// <param name="Title">The title of the shopping list.</param>
public record ShoppingListDto(Guid Id, string Title);
