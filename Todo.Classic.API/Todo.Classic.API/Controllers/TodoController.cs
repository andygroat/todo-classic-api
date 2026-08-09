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
    }
}
