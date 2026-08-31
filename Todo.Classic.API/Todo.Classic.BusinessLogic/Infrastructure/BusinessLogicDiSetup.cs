using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.BusinessLogic.Factories.Todos;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.BusinessLogic.Services.Todos;

namespace Todo.Classic.BusinessLogic.Infrastructure;

[ExcludeFromCodeCoverage]
public static class BusinessLogicDiSetup
{
    /// <summary>
    /// Adds business logic services to the IServiceCollection for dependency injection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services)
    {
        // Register business logic factories
        services.AddScoped<ITodoItemFactory, TodoItemFactory>();
        services.AddScoped<IShoppingListFactory, ShoppingListFactory>();
        services.AddScoped<IShoppingListItemFactory, ShoppingListItemFactory>();
        // Register business logic services
        services.AddScoped<ITodoService, TodoService>();
        services.AddScoped<IShoppingListService, ShoppingListService>();
        services.AddScoped<IShoppingListItemService, ShoppingListItemService>();

        return services;
    }
}
