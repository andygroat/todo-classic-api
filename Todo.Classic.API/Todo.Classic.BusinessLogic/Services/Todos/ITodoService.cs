using System;
using System.Collections.Generic;
using System.Text;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Services.Todos
{
    /// <summary>
    /// Defines the contract for a service that manages todo items.
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// Creates a new todo item.
        /// </summary>
        /// <param name="request">The request containing the details of the todo item to create.</param>
        /// <returns>The unique identifier of the created todo item.</returns>
        Task<Guid> CreateTodoItemAsync(CreateTodoRequest request);

        /// <summary>
        /// Gets todo items, optionally filtered by a search string matching the description.
        /// </summary>
        /// <param name="search">An optional search string to filter todo items by description.</param>
        /// <returns>A collection of todo items matching the search criteria.</returns>
        Task<IReadOnlyList<TodoItemDto>> GetTodoItemsAsync(string? search = null);

        /// <summary>
        /// Gets a todo item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the todo item.</param>
        /// <returns>The matching todo item, or <c>null</c> if not found.</returns>
        Task<TodoItemDto?> GetTodoItemByIdAsync(Guid id);

        /// <summary>
        /// Marks a todo item as completed.
        /// </summary>
        /// <param name="id">The unique identifier of the todo item to complete.</param>
        /// <returns>The updated todo item, or <c>null</c> if not found.</returns>
        Task<TodoItemDto?> CompleteTodoItemAsync(Guid id);
    }
}
