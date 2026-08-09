using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.Todos;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Factories.Todos
{
    /// <summary>
    /// A factory for creating instances of <see cref="TodoItem"/>.
    /// </summary>
    internal sealed class TodoItemFactory : ITodoItemFactory
    {
        /// <inheritdoc/>
        public TodoItem CreateTodoItem(CreateTodoRequest request)
        {
            // Validate the request object
            if (request == null)
                throw new ArgumentNullException(nameof(request), "The request object cannot be null.");
            if (string.IsNullOrWhiteSpace(request.Description))
                throw new BusinessLogicException("The description cannot be null or empty.");
            if (request.Description.Length > 100)
                throw new BusinessLogicException("The description cannot exceed 100 characters.");
            if (request.DueDate.HasValue && request.DueDate.Value < DateTime.UtcNow)
                throw new BusinessLogicException("The due date cannot be in the past.");

            // Create and return a new TodoItem instance
            return new TodoItem
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                DueDate = request.DueDate,
                IsCompleted = false
            };
        }
    }
}
