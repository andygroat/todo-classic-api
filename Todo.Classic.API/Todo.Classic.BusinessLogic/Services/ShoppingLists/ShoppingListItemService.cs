using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Services.ShoppingLists;

/// <summary>
/// Represents a service that manages shopping list items.
/// </summary>
public sealed class ShoppingListItemService(
    IShoppingListItemFactory shoppingListItemFactory,
    TodoDbContext todoDbContext,
    ILogger<ShoppingListItemService> logger) : IShoppingListItemService
{
    /// <inheritdoc/>
    public async Task<Guid?> CreateShoppingListItemAsync(Guid shoppingListId, CreateShoppingListItemRequest request)
    {
        try
        {
            logger.LogInformation("CreateShoppingListItemAsync method called at {Time} for {ShoppingListId}", DateTime.UtcNow, shoppingListId);

            var listExists = await todoDbContext.ShoppingLists.AnyAsync(l => l.Id == shoppingListId);
            if (!listExists)
                return null;

            var item = shoppingListItemFactory.CreateShoppingListItem(shoppingListId, request);
            todoDbContext.ShoppingListItems.Add(item);
            await todoDbContext.SaveChangesAsync();

            return item.Id;
        }
        finally
        {
            logger.LogInformation("CreateShoppingListItemAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ShoppingListItemDto>?> GetShoppingListItemsAsync(Guid shoppingListId)
    {
        try
        {
            logger.LogInformation("GetShoppingListItemsAsync method called at {Time} for {ShoppingListId}", DateTime.UtcNow, shoppingListId);

            var listExists = await todoDbContext.ShoppingLists.AnyAsync(l => l.Id == shoppingListId);
            if (!listExists)
                return null;

            return await todoDbContext.ShoppingListItems
                .Where(i => i.ShoppingListId == shoppingListId)
                .Select(i => new ShoppingListItemDto(i.Id, i.ShoppingListId, i.Title, i.IsComplete))
                .ToListAsync();
        }
        finally
        {
            logger.LogInformation("GetShoppingListItemsAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<ShoppingListItemDto?> GetShoppingListItemByIdAsync(Guid shoppingListId, Guid itemId)
    {
        try
        {
            logger.LogInformation("GetShoppingListItemByIdAsync method called at {Time} for {ShoppingListId}/{ItemId}", DateTime.UtcNow, shoppingListId, itemId);

            return await todoDbContext.ShoppingListItems
                .Where(i => i.ShoppingListId == shoppingListId && i.Id == itemId)
                .Select(i => new ShoppingListItemDto(i.Id, i.ShoppingListId, i.Title, i.IsComplete))
                .SingleOrDefaultAsync();
        }
        finally
        {
            logger.LogInformation("GetShoppingListItemByIdAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<ShoppingListItemDto?> UpdateShoppingListItemAsync(Guid shoppingListId, Guid itemId, UpdateShoppingListItemRequest request)
    {
        try
        {
            logger.LogInformation("UpdateShoppingListItemAsync method called at {Time} for {ShoppingListId}/{ItemId}", DateTime.UtcNow, shoppingListId, itemId);

            if (request == null)
                throw new ArgumentNullException(nameof(request), "The request object cannot be null.");
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new BusinessLogicException("The title cannot be null or empty.");
            if (request.Title.Length > 200)
                throw new BusinessLogicException("The title cannot exceed 200 characters.");

            var item = await todoDbContext.ShoppingListItems
                .SingleOrDefaultAsync(i => i.ShoppingListId == shoppingListId && i.Id == itemId);
            if (item is null)
                return null;

            item.Title = request.Title;
            await todoDbContext.SaveChangesAsync();

            return new ShoppingListItemDto(item.Id, item.ShoppingListId, item.Title, item.IsComplete);
        }
        finally
        {
            logger.LogInformation("UpdateShoppingListItemAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<ShoppingListItemDto?> CompleteShoppingListItemAsync(Guid shoppingListId, Guid itemId)
    {
        try
        {
            logger.LogInformation("CompleteShoppingListItemAsync method called at {Time} for {ShoppingListId}/{ItemId}", DateTime.UtcNow, shoppingListId, itemId);

            var item = await todoDbContext.ShoppingListItems
                .SingleOrDefaultAsync(i => i.ShoppingListId == shoppingListId && i.Id == itemId);
            if (item is null)
                return null;

            if (item.IsComplete)
                throw new BusinessLogicException("The shopping list item is already complete.");

            item.IsComplete = true;
            await todoDbContext.SaveChangesAsync();

            return new ShoppingListItemDto(item.Id, item.ShoppingListId, item.Title, item.IsComplete);
        }
        finally
        {
            logger.LogInformation("CompleteShoppingListItemAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteShoppingListItemAsync(Guid shoppingListId, Guid itemId)
    {
        try
        {
            logger.LogInformation("DeleteShoppingListItemAsync method called at {Time} for {ShoppingListId}/{ItemId}", DateTime.UtcNow, shoppingListId, itemId);

            var item = await todoDbContext.ShoppingListItems
                .SingleOrDefaultAsync(i => i.ShoppingListId == shoppingListId && i.Id == itemId);
            if (item is null)
                return false;

            todoDbContext.ShoppingListItems.Remove(item);
            await todoDbContext.SaveChangesAsync();
            return true;
        }
        finally
        {
            logger.LogInformation("DeleteShoppingListItemAsync method executed at {Time}", DateTime.UtcNow);
        }
    }
}
