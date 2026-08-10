using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Todo.Classic.BusinessLogic.Factories.Todos;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Services.Todos
{
    /// <summary>
    /// Represents a service that manages todo items.
    /// </summary>
    public sealed class TodoService(ITodoItemFactory todoItemFactory, TodoDbContext todoDbContext, ILogger<TodoService> logger) : ITodoService
    {
        /// <inheritdoc/>
        public async Task<Guid> CreateTodoItemAsync(CreateTodoRequest request)
        {
            try
            {
                logger.LogInformation("CreateTodoItemAsync method called at {Time}", DateTime.UtcNow);

                // Validate the request & create a new todo item using the factory
                var todoItem = todoItemFactory.CreateTodoItem(request);
                // Add the new todo item to the database context
                todoDbContext.TodoItems.Add(todoItem);
                await todoDbContext.SaveChangesAsync();

                return todoItem.Id;
            }
            finally
            {
                logger.LogInformation("CreateTodoItemAsync method executed at {Time}", DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyList<TodoItemDto>> GetTodoItemsAsync(string? search = null)
        {
            try
            {
                logger.LogInformation("GetTodoItemsAsync method called at {Time}", DateTime.UtcNow);

                // Start with the base query for todo items
                var query = todoDbContext.TodoItems.AsQueryable();
                // If a search term is provided, filter the query to include only items that match the search term in their description
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim();
                    query = query.Where(t => EF.Functions.Like(t.Description, $"%{term}%"));
                }
                // Execute the query and project the results into TodoItemDto objects
                return await query
                    .Select(t => new TodoItemDto(t.Id, t.Description, t.DueDate, t.IsCompleted, t.CompletedDate))
                    .ToListAsync();
            }
            finally
            {
                logger.LogInformation("GetTodoItemsAsync method executed at {Time}", DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<TodoItemDto?> GetTodoItemByIdAsync(Guid id)
        {
            try
            {
                logger.LogInformation("GetTodoItemByIdAsync method called at {Time}", DateTime.UtcNow);

                // Query the database for a todo item with the specified ID and project it into a TodoItemDto object
                return await todoDbContext.TodoItems
                    .Where(t => t.Id == id)
                    .Select(t => new TodoItemDto(t.Id, t.Description, t.DueDate, t.IsCompleted, t.CompletedDate))
                    .SingleOrDefaultAsync();
            }
            finally
            {
                logger.LogInformation("GetTodoItemByIdAsync method executed at {Time}", DateTime.UtcNow);
            }
        }

        /// <inheritdoc/>
        public async Task<TodoItemDto?> CompleteTodoItemAsync(Guid id)
        {
            try
            {
                logger.LogInformation("CompleteTodoItemAsync method called at {Time}", DateTime.UtcNow);

                // Query the database for a todo item with the specified ID
                var todoItem = await todoDbContext.TodoItems.SingleOrDefaultAsync(t => t.Id == id);
                // If the todo item is not found, return null
                if (todoItem is null)
                    return null;

                // If the todo item is already completed, throw a BusinessLogicException
                if (todoItem.IsCompleted)
                    throw new BusinessLogicException("The todo item is already completed.");

                // Mark the todo item as completed and set the completed date to the current UTC time
                todoItem.IsCompleted = true;
                todoItem.CompletedDate = DateTime.UtcNow;

                await todoDbContext.SaveChangesAsync();

                return new TodoItemDto(todoItem.Id, todoItem.Description, todoItem.DueDate, todoItem.IsCompleted, todoItem.CompletedDate);
            }
            finally
            {
                logger.LogInformation("CompleteTodoItemAsync method executed at {Time}", DateTime.UtcNow);
            }
        }
    }
}
