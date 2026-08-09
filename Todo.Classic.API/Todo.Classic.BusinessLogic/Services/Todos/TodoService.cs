using Microsoft.Extensions.Logging;
using Todo.Classic.BusinessLogic.Factories.Todos;
using Todo.Classic.DataAccess.Context;
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
    }
}
