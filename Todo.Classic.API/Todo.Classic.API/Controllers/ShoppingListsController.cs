using Microsoft.AspNetCore.Mvc;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingListsController(
        ILogger<ShoppingListsController> logger,
        IShoppingListService shoppingListService) : ControllerBase
    {
        /// <summary>
        /// Creates a new shopping list.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Description = "Shopping list created successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> CreateShoppingList(CreateShoppingListRequest request)
        {
            try
            {
                logger.LogInformation("Received CreateShoppingList request: {@Request}", request);
                var createdId = await shoppingListService.CreateShoppingListAsync(request);
                return Created($"/api/shoppinglists/{createdId}", new { id = createdId });
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while creating a shopping list.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a shopping list.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("CreateShoppingList request completed.");
            }
        }

        /// <summary>
        /// Gets shopping lists, optionally filtered by a search string.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ShoppingListDto>), StatusCodes.Status200OK, Description = "Shopping lists retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetShoppingLists([FromQuery] string? search = null)
        {
            try
            {
                logger.LogInformation("Received GetShoppingLists request with search: {Search}", search);
                var lists = await shoppingListService.GetShoppingListsAsync(search);
                return Ok(lists);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving shopping lists.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetShoppingLists request completed.");
            }
        }

        /// <summary>
        /// Gets a shopping list by its unique identifier.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK, Description = "Shopping list retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetShoppingListById(Guid id)
        {
            try
            {
                logger.LogInformation("Received GetShoppingListById request for id: {ShoppingListId}", id);
                var list = await shoppingListService.GetShoppingListByIdAsync(id);
                if (list is null)
                {
                    return NotFound();
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the shopping list.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetShoppingListById request completed.");
            }
        }

        /// <summary>
        /// Updates the title of an existing shopping list.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ShoppingListDto), StatusCodes.Status200OK, Description = "Shopping list updated successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> UpdateShoppingList(Guid id, UpdateShoppingListRequest request)
        {
            try
            {
                logger.LogInformation("Received UpdateShoppingList request for id: {ShoppingListId} {@Request}", id, request);
                var updated = await shoppingListService.UpdateShoppingListAsync(id, request);
                if (updated is null)
                {
                    return NotFound();
                }
                return Ok(updated);
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while updating a shopping list.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the shopping list.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("UpdateShoppingList request completed.");
            }
        }

        /// <summary>
        /// Deletes a shopping list and all of its items.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Description = "Shopping list deleted successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> DeleteShoppingList(Guid id)
        {
            try
            {
                logger.LogInformation("Received DeleteShoppingList request for id: {ShoppingListId}", id);
                var deleted = await shoppingListService.DeleteShoppingListAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting the shopping list.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("DeleteShoppingList request completed.");
            }
        }
    }
}
