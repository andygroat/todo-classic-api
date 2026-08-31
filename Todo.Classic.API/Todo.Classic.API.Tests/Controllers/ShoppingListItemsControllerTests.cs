using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Todo.Classic.API.Controllers;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.API.Tests.Controllers
{
    [TestFixture]
    public class ShoppingListItemsControllerTests
    {
        private ILogger<ShoppingListItemsController> logger;
        private IShoppingListItemService service;
        private ShoppingListItemsController controller;

        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger<ShoppingListItemsController>>();
            service = Substitute.For<IShoppingListItemService>();
            controller = new ShoppingListItemsController(logger, service);
        }

        [Test]
        public async Task CreateShoppingListItem_Success_ReturnsCreated()
        {
            var listId = Guid.NewGuid();
            var request = new CreateShoppingListItemRequest("Milk");
            var itemId = Guid.NewGuid();
            service.CreateShoppingListItemAsync(listId, request).Returns(itemId);

            var result = await controller.CreateShoppingListItem(listId, request);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
        }

        [Test]
        public async Task CreateShoppingListItem_ListMissing_ReturnsNotFound()
        {
            var listId = Guid.NewGuid();
            var request = new CreateShoppingListItemRequest("Milk");
            service.CreateShoppingListItemAsync(listId, request).Returns((Guid?)null);

            var result = await controller.CreateShoppingListItem(listId, request);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CreateShoppingListItem_BusinessLogicException_ReturnsBadRequest()
        {
            var listId = Guid.NewGuid();
            var request = new CreateShoppingListItemRequest("bad");
            service.CreateShoppingListItemAsync(listId, request)
                .Returns(Task.FromException<Guid?>(new BusinessLogicException("invalid")));

            var result = await controller.CreateShoppingListItem(listId, request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateShoppingListItem_Exception_Returns500()
        {
            var listId = Guid.NewGuid();
            var request = new CreateShoppingListItemRequest("Milk");
            service.CreateShoppingListItemAsync(listId, request)
                .Returns(Task.FromException<Guid?>(new Exception("boom")));

            var result = await controller.CreateShoppingListItem(listId, request);

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetShoppingListItems_Found_ReturnsOk()
        {
            var listId = Guid.NewGuid();
            var items = new List<ShoppingListItemDto> { new(Guid.NewGuid(), listId, "Milk", false) };
            service.GetShoppingListItemsAsync(listId).Returns(items);

            var result = await controller.GetShoppingListItems(listId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(items));
        }

        [Test]
        public async Task GetShoppingListItems_ListMissing_ReturnsNotFound()
        {
            var listId = Guid.NewGuid();
            service.GetShoppingListItemsAsync(listId).Returns((IReadOnlyList<ShoppingListItemDto>?)null);

            var result = await controller.GetShoppingListItems(listId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetShoppingListItemById_Found_ReturnsOk()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var dto = new ShoppingListItemDto(itemId, listId, "Milk", false);
            service.GetShoppingListItemByIdAsync(listId, itemId).Returns(dto);

            var result = await controller.GetShoppingListItemById(listId, itemId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetShoppingListItemById_NotFound()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            service.GetShoppingListItemByIdAsync(listId, itemId).Returns((ShoppingListItemDto?)null);

            var result = await controller.GetShoppingListItemById(listId, itemId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateShoppingListItem_Found_ReturnsOk()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var request = new UpdateShoppingListItemRequest("New");
            var dto = new ShoppingListItemDto(itemId, listId, "New", false);
            service.UpdateShoppingListItemAsync(listId, itemId, request).Returns(dto);

            var result = await controller.UpdateShoppingListItem(listId, itemId, request);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateShoppingListItem_NotFound()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var request = new UpdateShoppingListItemRequest("New");
            service.UpdateShoppingListItemAsync(listId, itemId, request).Returns((ShoppingListItemDto?)null);

            var result = await controller.UpdateShoppingListItem(listId, itemId, request);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateShoppingListItem_BusinessLogicException_ReturnsBadRequest()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var request = new UpdateShoppingListItemRequest("bad");
            service.UpdateShoppingListItemAsync(listId, itemId, request)
                .Returns(Task.FromException<ShoppingListItemDto?>(new BusinessLogicException("invalid")));

            var result = await controller.UpdateShoppingListItem(listId, itemId, request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CompleteShoppingListItem_Found_ReturnsOk()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var dto = new ShoppingListItemDto(itemId, listId, "Milk", true);
            service.CompleteShoppingListItemAsync(listId, itemId).Returns(dto);

            var result = await controller.CompleteShoppingListItem(listId, itemId);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task CompleteShoppingListItem_NotFound()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            service.CompleteShoppingListItemAsync(listId, itemId).Returns((ShoppingListItemDto?)null);

            var result = await controller.CompleteShoppingListItem(listId, itemId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CompleteShoppingListItem_AlreadyComplete_ReturnsBadRequest()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            service.CompleteShoppingListItemAsync(listId, itemId)
                .Returns(Task.FromException<ShoppingListItemDto?>(new BusinessLogicException("already complete")));

            var result = await controller.CompleteShoppingListItem(listId, itemId);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteShoppingListItem_Found_ReturnsNoContent()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            service.DeleteShoppingListItemAsync(listId, itemId).Returns(true);

            var result = await controller.DeleteShoppingListItem(listId, itemId);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteShoppingListItem_NotFound()
        {
            var listId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            service.DeleteShoppingListItemAsync(listId, itemId).Returns(false);

            var result = await controller.DeleteShoppingListItem(listId, itemId);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
