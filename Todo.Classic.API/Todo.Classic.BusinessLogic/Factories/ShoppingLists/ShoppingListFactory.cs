using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Factories.ShoppingLists;

/// <summary>
/// A factory for creating instances of <see cref="ShoppingList"/>.
/// </summary>
internal sealed class ShoppingListFactory : IShoppingListFactory
{
    /// <inheritdoc/>
    public ShoppingList CreateShoppingList(CreateShoppingListRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "The request object cannot be null.");
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new BusinessLogicException("The title cannot be null or empty.");
        if (request.Title.Length > 200)
            throw new BusinessLogicException("The title cannot exceed 200 characters.");

        return new ShoppingList
        {
            Id = Guid.NewGuid(),
            Title = request.Title
        };
    }
}
