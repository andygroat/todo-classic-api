using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Factories.ShoppingLists;

/// <summary>
/// A factory for creating instances of <see cref="ShoppingListItem"/>.
/// </summary>
internal sealed class ShoppingListItemFactory : IShoppingListItemFactory
{
    /// <inheritdoc/>
    public ShoppingListItem CreateShoppingListItem(Guid shoppingListId, CreateShoppingListItemRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "The request object cannot be null.");
        if (shoppingListId == Guid.Empty)
            throw new BusinessLogicException("The shopping list id cannot be empty.");
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BusinessLogicException("The title cannot be null or empty.");
        if (request.Title.Length > 200)
            throw new BusinessLogicException("The title cannot exceed 200 characters.");

        return new ShoppingListItem
        {
            Id = Guid.NewGuid(),
            ShoppingListId = shoppingListId,
            Title = request.Title,
            IsComplete = false
        };
    }
}
