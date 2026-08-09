using System;
using System.Collections.Generic;
using System.Text;

namespace Todo.Classic.Model.DTO.Todos
{
    /// <summary>
    /// Represents a request to create a new Todo item.
    /// </summary>
    /// <param name="Description">The description of the Todo item.</param>
    /// <param name="DueDate">The due date of the Todo item.</param>
    public record CreateTodoRequest(string Description, DateTime? DueDate);
}
