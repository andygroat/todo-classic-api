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
    }
}
