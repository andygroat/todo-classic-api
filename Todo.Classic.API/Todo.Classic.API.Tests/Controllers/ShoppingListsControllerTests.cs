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
    public class ShoppingListsControllerTests
    {
        private ILogger<ShoppingListsController> logger;
        private IShoppingListService service;
        private ShoppingListsController controller;

        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger<ShoppingListsController>>();
            service = Substitute.For<IShoppingListService>();
            controller = new ShoppingListsController(logger, service);
        }

        [Test]
        public async Task CreateShoppingList_Success_ReturnsCreated()
        {
            var request = new CreateShoppingListRequest("Groceries");
            var id = Guid.NewGuid();
            service.CreateShoppingListAsync(request).Returns(id);

            var result = await controller.CreateShoppingList(request);

            Assert.That(result, Is.InstanceOf<CreatedResult>());
        }

        [Test]
        public async Task CreateShoppingList_BusinessLogicException_ReturnsBadRequest()
        {
            var request = new CreateShoppingListRequest("bad");
            service.CreateShoppingListAsync(request).Returns(Task.FromException<Guid>(new BusinessLogicException("invalid")));

            var result = await controller.CreateShoppingList(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateShoppingList_Exception_Returns500()
        {
            var request = new CreateShoppingListRequest("Groceries");
            service.CreateShoppingListAsync(request).Returns(Task.FromException<Guid>(new Exception("boom")));

            var result = await controller.CreateShoppingList(request);

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetShoppingLists_ReturnsOk()
        {
            var lists = new List<ShoppingListDto>
            {
                new(Guid.NewGuid(), "First")
            };
            service.GetShoppingListsAsync(null).Returns(lists);

            var result = await controller.GetShoppingLists();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(lists));
        }

        [Test]
        public async Task GetShoppingLists_Exception_Returns500()
        {
            service.GetShoppingListsAsync(Arg.Any<string?>())
                .Returns(Task.FromException<IReadOnlyList<ShoppingListDto>>(new Exception("boom")));

            var result = await controller.GetShoppingLists();

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetShoppingListById_Found_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var dto = new ShoppingListDto(id, "List");
            service.GetShoppingListByIdAsync(id).Returns(dto);

            var result = await controller.GetShoppingListById(id);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(dto));
        }

        [Test]
        public async Task GetShoppingListById_NotFound()
        {
            var id = Guid.NewGuid();
            service.GetShoppingListByIdAsync(id).Returns((ShoppingListDto?)null);

            var result = await controller.GetShoppingListById(id);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetShoppingListById_Exception_Returns500()
        {
            var id = Guid.NewGuid();
            service.GetShoppingListByIdAsync(id).Returns(Task.FromException<ShoppingListDto?>(new Exception("boom")));

            var result = await controller.GetShoppingListById(id);

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateShoppingList_Found_ReturnsOk()
        {
            var id = Guid.NewGuid();
            var request = new UpdateShoppingListRequest("New");
            var dto = new ShoppingListDto(id, "New");
            service.UpdateShoppingListAsync(id, request).Returns(dto);

            var result = await controller.UpdateShoppingList(id, request);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task UpdateShoppingList_NotFound()
        {
            var id = Guid.NewGuid();
            var request = new UpdateShoppingListRequest("New");
            service.UpdateShoppingListAsync(id, request).Returns((ShoppingListDto?)null);

            var result = await controller.UpdateShoppingList(id, request);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateShoppingList_BusinessLogicException_ReturnsBadRequest()
        {
            var id = Guid.NewGuid();
            var request = new UpdateShoppingListRequest("bad");
            service.UpdateShoppingListAsync(id, request)
                .Returns(Task.FromException<ShoppingListDto?>(new BusinessLogicException("invalid")));

            var result = await controller.UpdateShoppingList(id, request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task DeleteShoppingList_Found_ReturnsNoContent()
        {
            var id = Guid.NewGuid();
            service.DeleteShoppingListAsync(id).Returns(true);

            var result = await controller.DeleteShoppingList(id);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteShoppingList_NotFound()
        {
            var id = Guid.NewGuid();
            service.DeleteShoppingListAsync(id).Returns(false);

            var result = await controller.DeleteShoppingList(id);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteShoppingList_Exception_Returns500()
        {
            var id = Guid.NewGuid();
            service.DeleteShoppingListAsync(id).Returns(Task.FromException<bool>(new Exception("boom")));

            var result = await controller.DeleteShoppingList(id);

            Assert.That(result, Is.InstanceOf<StatusCodeResult>());
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }
    }
}
