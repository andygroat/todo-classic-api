using Microsoft.AspNetCore.Mvc;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.API.Controllers
{
    [Route("api/shoppinglists/{listId:guid}/items")]
    [ApiController]
    public class ShoppingListItemsController(
        ILogger<ShoppingListItemsController> logger,
        IShoppingListItemService shoppingListItemService) : ControllerBase
    {
        /// <summary>
        /// Creates a new shopping list item on the specified shopping list.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Description = "Shopping list item created successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> CreateShoppingListItem(Guid listId, CreateShoppingListItemRequest request)
        {
            try
            {
                logger.LogInformation("Received CreateShoppingListItem request for {ShoppingListId}: {@Request}", listId, request);
                var createdId = await shoppingListItemService.CreateShoppingListItemAsync(listId, request);
                if (createdId is null)
                {
                    return NotFound();
                }
                return Created($"/api/shoppinglists/{listId}/items/{createdId}", new { id = createdId });
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while creating a shopping list item.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while creating a shopping list item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("CreateShoppingListItem request completed.");
            }
        }

        /// <summary>
        /// Gets items for the specified shopping list.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ShoppingListItemDto>), StatusCodes.Status200OK, Description = "Shopping list items retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetShoppingListItems(Guid listId)
        {
            try
            {
                logger.LogInformation("Received GetShoppingListItems request for {ShoppingListId}", listId);
                var items = await shoppingListItemService.GetShoppingListItemsAsync(listId);
                if (items is null)
                {
                    return NotFound();
                }
                return Ok(items);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving shopping list items.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetShoppingListItems request completed.");
            }
        }

        /// <summary>
        /// Gets a shopping list item by its unique identifier.
        /// </summary>
        [HttpGet("{itemId:guid}")]
        [ProducesResponseType(typeof(ShoppingListItemDto), StatusCodes.Status200OK, Description = "Shopping list item retrieved successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> GetShoppingListItemById(Guid listId, Guid itemId)
        {
            try
            {
                logger.LogInformation("Received GetShoppingListItemById request for {ShoppingListId}/{ItemId}", listId, itemId);
                var item = await shoppingListItemService.GetShoppingListItemByIdAsync(listId, itemId);
                if (item is null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while retrieving the shopping list item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("GetShoppingListItemById request completed.");
            }
        }

        /// <summary>
        /// Updates the title of an existing shopping list item.
        /// </summary>
        [HttpPut("{itemId:guid}")]
        [ProducesResponseType(typeof(ShoppingListItemDto), StatusCodes.Status200OK, Description = "Shopping list item updated successfully.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> UpdateShoppingListItem(Guid listId, Guid itemId, UpdateShoppingListItemRequest request)
        {
            try
            {
                logger.LogInformation("Received UpdateShoppingListItem request for {ShoppingListId}/{ItemId}: {@Request}", listId, itemId, request);
                var updated = await shoppingListItemService.UpdateShoppingListItemAsync(listId, itemId, request);
                if (updated is null)
                {
                    return NotFound();
                }
                return Ok(updated);
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while updating a shopping list item.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while updating the shopping list item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("UpdateShoppingListItem request completed.");
            }
        }

        /// <summary>
        /// Marks a shopping list item as complete.
        /// </summary>
        [HttpPost("{itemId:guid}/complete")]
        [ProducesResponseType(typeof(ShoppingListItemDto), StatusCodes.Status200OK, Description = "Shopping list item marked complete.")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Description = "Invalid request.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> CompleteShoppingListItem(Guid listId, Guid itemId)
        {
            try
            {
                logger.LogInformation("Received CompleteShoppingListItem request for {ShoppingListId}/{ItemId}", listId, itemId);
                var item = await shoppingListItemService.CompleteShoppingListItemAsync(listId, itemId);
                if (item is null)
                {
                    return NotFound();
                }
                return Ok(item);
            }
            catch (BusinessLogicException bex)
            {
                logger.LogWarning(bex, "Business logic exception occurred while completing a shopping list item.");
                return BadRequest(bex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while completing the shopping list item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("CompleteShoppingListItem request completed.");
            }
        }

        /// <summary>
        /// Deletes a shopping list item.
        /// </summary>
        [HttpDelete("{itemId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent, Description = "Shopping list item deleted successfully.")]
        [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Shopping list item not found.")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Description = "Internal server error.")]
        public async Task<IActionResult> DeleteShoppingListItem(Guid listId, Guid itemId)
        {
            try
            {
                logger.LogInformation("Received DeleteShoppingListItem request for {ShoppingListId}/{ItemId}", listId, itemId);
                var deleted = await shoppingListItemService.DeleteShoppingListItemAsync(listId, itemId);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while deleting the shopping list item.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            finally
            {
                logger.LogInformation("DeleteShoppingListItem request completed.");
            }
        }
    }
}
