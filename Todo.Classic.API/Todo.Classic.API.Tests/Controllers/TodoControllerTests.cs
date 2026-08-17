using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Todo.Classic.API.Controllers;
using Todo.Classic.BusinessLogic.Services.Todos;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.API.Tests.Controllers
{
    [TestFixture]
    public class TodoControllerTests
    {
        // Dependencies for the TodoController
        private ILogger<TodoController> logger;
        private ITodoService todoService;

        // The instance of the TodoController being tested
        private TodoController controller;

        [SetUp] public void SetUp() 
        {
            // Initialize logger and todoService with mock implementations using NSubstitute
            logger = Substitute.For<ILogger<TodoController>>();
            todoService = Substitute.For<ITodoService>();

            // Create an instance of the TodoController with the mocked dependencies
            controller = new TodoController(logger, todoService);
        }


        [Test]
        public async Task CreateTodo()
        {
            // Arrange
            var createTodoRequest = new CreateTodoRequest("This is a test todo item.", null);
            var resultTodoItemId = Guid.NewGuid();

            // Expectations
            todoService.CreateTodoItemAsync(createTodoRequest).Returns(resultTodoItemId);

            // Act
            var result = await controller.CreateTodo(createTodoRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<CreatedResult>());
            Received.InOrder(async () =>
            {
                await todoService.CreateTodoItemAsync(createTodoRequest);
            });
        }

        [Test]
        public async Task CreateTodo_BusinessLogicException()
        {
            // Arrange
            var createTodoRequest = new CreateTodoRequest("This is a test todo item.", null);
            var expectedBusinessLogicException = new BusinessLogicException("Business logic error occurred.");

            // Expectations
            todoService.CreateTodoItemAsync(createTodoRequest).Returns(Task.FromException<Guid>(expectedBusinessLogicException));

            // Act
            var result = await controller.CreateTodo(createTodoRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateTodo_Exception()
        {
            // Arrange
            var createTodoRequest = new CreateTodoRequest("This is a test todo item.", null);
            var expectedException = new Exception("Test exception occurred.");

            // Expectations
            todoService.CreateTodoItemAsync(createTodoRequest).Returns(Task.FromException<Guid>(expectedException));

            // Act
            var result = await controller.CreateTodo(createTodoRequest);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(StatusCodeResult)));
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetTodos()
        {
            // Arrange
            var expectedTodos = new List<TodoItemDto>
            {
                new(Guid.NewGuid(), "First", null, false, null),
                new(Guid.NewGuid(), "Second", DateTime.UtcNow.AddDays(1), true, DateTime.UtcNow)
            };

            // Expectations
            todoService.GetTodoItemsAsync(null).Returns(expectedTodos);

            // Act
            var result = await controller.GetTodos();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expectedTodos));
        }

        [Test]
        public async Task GetTodos_WithSearch()
        {
            // Arrange
            const string search = "first";
            var expectedTodos = new List<TodoItemDto>
            {
                new(Guid.NewGuid(), "First", null, false, null)
            };

            // Expectations
            todoService.GetTodoItemsAsync(search).Returns(expectedTodos);

            // Act
            var result = await controller.GetTodos(search);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expectedTodos));
            await todoService.Received(1).GetTodoItemsAsync(search);
        }

        [Test]
        public async Task GetTodos_Exception()
        {
            // Arrange
            var expectedException = new Exception("Test exception occurred.");

            // Expectations
            todoService.GetTodoItemsAsync(Arg.Any<string?>()).Returns(Task.FromException<IReadOnlyList<TodoItemDto>>(expectedException));

            // Act
            var result = await controller.GetTodos();

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(StatusCodeResult)));
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetTodoById()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedTodo = new TodoItemDto(id, "A todo", null, false, null);

            // Expectations
            todoService.GetTodoItemByIdAsync(id).Returns(expectedTodo);

            // Act
            var result = await controller.GetTodoById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expectedTodo));
        }

        [Test]
        public async Task GetTodoById_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Expectations
            todoService.GetTodoItemByIdAsync(id).Returns((TodoItemDto?)null);

            // Act
            var result = await controller.GetTodoById(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task GetTodoById_Exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedException = new Exception("Test exception occurred.");

            // Expectations
            todoService.GetTodoItemByIdAsync(id).Returns(Task.FromException<TodoItemDto?>(expectedException));

            // Act
            var result = await controller.GetTodoById(id);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(StatusCodeResult)));
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CompleteTodo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedTodo = new TodoItemDto(id, "A todo", null, true, DateTime.UtcNow);

            // Expectations
            todoService.CompleteTodoItemAsync(id).Returns(expectedTodo);

            // Act
            var result = await controller.CompleteTodo(id);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.EqualTo(expectedTodo));
        }

        [Test]
        public async Task CompleteTodo_NotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Expectations
            todoService.CompleteTodoItemAsync(id).Returns((TodoItemDto?)null);

            // Act
            var result = await controller.CompleteTodo(id);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task CompleteTodo_BusinessLogicException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedBusinessLogicException = new BusinessLogicException("Business logic error occurred.");

            // Expectations
            todoService.CompleteTodoItemAsync(id).Returns(Task.FromException<TodoItemDto?>(expectedBusinessLogicException));

            // Act
            var result = await controller.CompleteTodo(id);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CompleteTodo_Exception()
        {
            // Arrange
            var id = Guid.NewGuid();
            var expectedException = new Exception("Test exception occurred.");

            // Expectations
            todoService.CompleteTodoItemAsync(id).Returns(Task.FromException<TodoItemDto?>(expectedException));

            // Act
            var result = await controller.CompleteTodo(id);

            // Assert
            Assert.That(result, Is.InstanceOf(typeof(StatusCodeResult)));
            Assert.That(((StatusCodeResult)result).StatusCode, Is.EqualTo(500));
        }

    }
}
