using Microsoft.AspNetCore.Mvc;
using Todo.Classic.BusinessLogic.Services.Todos;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController (ILogger<TodoController> logger, ITodoService todoService) : ControllerBase
    {
        /// <summary>
        /// Creates a new Todo item.
        /// </summary>
        /// <param name="request">The request object containing the details of the Todo item to create.</param>
        /// <returns>An IActionResult representing the result of the operation.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Description = "Todo item created successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> CreateTodo(CreateTodoRequest request)
        {
            try
            {
                logger.LogInformation("Received CreateTodo request: {@Request}", request);
                // Create the Todo item using the service layer
                var createdTodoId = await todoService.CreateTodoItemAsync(request);
                // Return a 201 Created response with the location of the created resource
                return Created($"/api/todos/{createdTodoId}", new { id = createdTodoId });
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while creating a Todo item.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a Todo item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("CreateTodo request completed.");
            }
        }

        /// <summary>
        /// Gets todo items, optionally filtered by a search string.
        /// </summary>
        /// <param name="search">An optional search string to filter todo items by description.</param>
        /// <returns>A collection of todo items matching the search criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TodoItemDto>), StatusCodes.Status200OK, Description = "Todo items retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetTodos([FromQuery] string? search = null)
        {
            try
            {
                logger.LogInformation("Received GetTodos request with search: {Search}", search);
                var todos = await todoService.GetTodoItemsAsync(search);
                return Ok(todos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving Todo items.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetTodos request completed.");
            }
        }

        /// <summary>
        /// Gets a Todo item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Todo item.</param>
        /// <returns>The matching Todo item, or 404 if not found.</returns>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK, Description = "Todo item retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Todo item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetTodoById(Guid id)
        {
            try
            {
                logger.LogInformation("Received GetTodoById request for id: {Id}", id);
                var todo = await todoService.GetTodoItemByIdAsync(id);
                if (todo is null)
                {
                    return NotFound();
                }
                return Ok(todo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the Todo item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetTodoById request completed.");
            }
        }

        /// <summary>
        /// Marks a Todo item as completed.
        /// </summary>
        /// <param name="id">The unique identifier of the Todo item to complete.</param>
        /// <returns>The updated Todo item, or 404 if not found.</returns>
        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK, Description = "Todo item completed successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Todo item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> CompleteTodo(Guid id)
        {
            try
            {
                logger.LogInformation("Received CompleteTodo request for id: {Id}", id);
                var todo = await todoService.CompleteTodoItemAsync(id);
                if (todo is null)
                {
                    return NotFound();
                }
                return Ok(todo);
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while completing a Todo item.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while completing the Todo item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("CompleteTodo request completed.");
            }
        }
    }
}
