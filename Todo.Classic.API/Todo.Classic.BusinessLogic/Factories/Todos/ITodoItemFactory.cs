using Todo.Classic.Model.Domain.Todos;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Factories.Todos;

/// <summary>
/// Defines the contract for a factory that creates instances of <see cref="TodoItem"/>.
/// </summary>
public interface ITodoItemFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="TodoItem"/> based on the provided <see cref="CreateTodoRequest"/>.
    /// </summary>
    /// <param name="request">The request containing the details of the todo item to create.</param>
    /// <returns>A new instance of <see cref="TodoItem"/>.</returns>
    TodoItem CreateTodoItem (CreateTodoRequest request);
}
