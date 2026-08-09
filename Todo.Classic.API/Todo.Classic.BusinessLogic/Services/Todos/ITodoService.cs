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
    }
}
