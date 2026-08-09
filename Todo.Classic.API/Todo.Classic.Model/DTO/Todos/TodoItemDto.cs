namespace Todo.Classic.Model.DTO.Todos
{
    /// <summary>
    /// Represents a Todo item returned to clients.
    /// </summary>
    /// <param name="Id">The unique identifier of the Todo item.</param>
    /// <param name="Description">The description of the Todo item.</param>
    /// <param name="DueDate">The due date of the Todo item.</param>
    /// <param name="IsCompleted">Whether the Todo item is completed.</param>
    /// <param name="CompletedDate">The date the Todo item was completed.</param>
    public record TodoItemDto(Guid Id, string Description, DateTime? DueDate, bool IsCompleted, DateTime? CompletedDate);
}
