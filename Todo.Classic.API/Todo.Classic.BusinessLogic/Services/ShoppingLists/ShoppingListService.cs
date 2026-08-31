using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Services.ShoppingLists;

/// <summary>
/// Represents a service that manages shopping lists.
/// </summary>
public sealed class ShoppingListService(
    IShoppingListFactory shoppingListFactory,
    TodoDbContext todoDbContext,
    ILogger<ShoppingListService> logger) : IShoppingListService
{
    /// <inheritdoc/>
    public async Task<Guid> CreateShoppingListAsync(CreateShoppingListRequest request)
    {
        try
        {
            logger.LogInformation("CreateShoppingListAsync method called at {Time}", DateTime.UtcNow);

            var shoppingList = shoppingListFactory.CreateShoppingList(request);
            todoDbContext.ShoppingLists.Add(shoppingList);
            await todoDbContext.SaveChangesAsync();

            return shoppingList.Id;
        }
        finally
        {
            logger.LogInformation("CreateShoppingListAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ShoppingListDto>> GetShoppingListsAsync(string? search = null)
    {
        try
        {
            logger.LogInformation("GetShoppingListsAsync method called at {Time}", DateTime.UtcNow);

            var query = todoDbContext.ShoppingLists.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(l => EF.Functions.Like(l.Title, $"%{term}%"));
            }

            return await query
                .Select(l => new ShoppingListDto(l.Id, l.Title))
                .ToListAsync();
        }
        finally
        {
            logger.LogInformation("GetShoppingListsAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<ShoppingListDto?> GetShoppingListByIdAsync(Guid id)
    {
        try
        {
            logger.LogInformation("GetShoppingListByIdAsync method called at {Time} for {ShoppingListId}", DateTime.UtcNow, id);

            return await todoDbContext.ShoppingLists
                .Where(l => l.Id == id)
                .Select(l => new ShoppingListDto(l.Id, l.Title))
                .SingleOrDefaultAsync();
        }
        finally
        {
            logger.LogInformation("GetShoppingListByIdAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<ShoppingListDto?> UpdateShoppingListAsync(Guid id, UpdateShoppingListRequest request)
    {
        try
        {
            logger.LogInformation("UpdateShoppingListAsync method called at {Time} for {ShoppingListId}", DateTime.UtcNow, id);

            if (request == null)
                throw new ArgumentNullException(nameof(request), "The request object cannot be null.");
            if (string.IsNullOrWhiteSpace(request.Title))
                throw new BusinessLogicException("The title cannot be null or empty.");
            if (request.Title.Length > 200)
                throw new BusinessLogicException("The title cannot exceed 200 characters.");

            var shoppingList = await todoDbContext.ShoppingLists
                .Include(l => l.Items)
                .SingleOrDefaultAsync(l => l.Id == id);
            if (shoppingList is null)
                return null;

            shoppingList.Title = request.Title;
            await todoDbContext.SaveChangesAsync();

            return new ShoppingListDto(shoppingList.Id, shoppingList.Title);
        }
        finally
        {
            logger.LogInformation("UpdateShoppingListAsync method executed at {Time}", DateTime.UtcNow);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteShoppingListAsync(Guid id)
    {
        try
        {
            logger.LogInformation("DeleteShoppingListAsync method called at {Time} for {ShoppingListId}", DateTime.UtcNow, id);

            var shoppingList = await todoDbContext.ShoppingLists
                .Include(l => l.Items)
                .SingleOrDefaultAsync(l => l.Id == id);
            if (shoppingList is null)
                return false;

            // Explicitly remove children so cascade also works with the InMemory provider.
            if (shoppingList.Items.Count > 0)
                todoDbContext.ShoppingListItems.RemoveRange(shoppingList.Items);
            todoDbContext.ShoppingLists.Remove(shoppingList);

            await todoDbContext.SaveChangesAsync();
            return true;
        }
        finally
        {
            logger.LogInformation("DeleteShoppingListAsync method executed at {Time}", DateTime.UtcNow);
        }
    }
}
