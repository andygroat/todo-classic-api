using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Todo.Classic.BusinessLogic.Factories.Todos;
using Todo.Classic.BusinessLogic.Services.Todos;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.Todos;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Tests.Services.Todos
{
    [TestFixture]
    public class TodoServiceTests
    {
        private ITodoItemFactory todoItemFactory;
        private TodoDbContext dbContext;
        private ILogger<TodoService> logger;
        private TodoService service;

        /// <summary>
        /// Sets up the test environment before each test case.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Create substitutes for dependencies
            todoItemFactory = Substitute.For<ITodoItemFactory>();
            logger = Substitute.For<ILogger<TodoService>>();

            // Use an in-memory database for testing
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new TodoDbContext(options);

            // Initialize the service with the mocked dependencies
            service = new TodoService(todoItemFactory, dbContext, logger);
        }

        /// <summary>
        /// Seeds the in-memory database with the provided TodoItems.
        /// </summary>
        /// <param name="items"></param>
        private void SeedItems(params TodoItem[] items)
        {
            dbContext.TodoItems.AddRange(items);
            dbContext.SaveChanges();
        }

        /// <summary>
        /// Cleans up the test environment after each test case.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        [Test]
        public async Task CreateTodoItemAsync_ValidRequest_AddsItemAndReturnsId()
        {
            // Arrange
            var request = new CreateTodoRequest("A todo", null);
            var expectedItem = new TodoItem { Id = Guid.NewGuid(), Description = "A todo" };
            todoItemFactory.CreateTodoItem(request).Returns(expectedItem);

            // Act
            var result = await service.CreateTodoItemAsync(request);

            // Assert
            Assert.That(result, Is.EqualTo(expectedItem.Id));
            Assert.That(dbContext.TodoItems.Count(), Is.EqualTo(1));
            Assert.That(dbContext.TodoItems.Single().Id, Is.EqualTo(expectedItem.Id));
        }

        [Test]
        public void CreateTodoItemAsync_FactoryThrowsBusinessLogicException_Propagates()
        {
            // Arrange
            var request = new CreateTodoRequest("bad", null);
            todoItemFactory.CreateTodoItem(request).Throws(new BusinessLogicException("invalid"));

            // Act & Assert
            Assert.That(async () => await service.CreateTodoItemAsync(request),
                Throws.TypeOf<BusinessLogicException>());
            Assert.That(dbContext.TodoItems.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetTodoItemsAsync_NoSearch_ReturnsAllItems()
        {
            // Arrange
            SeedItems(
                new TodoItem { Id = Guid.NewGuid(), Description = "First" },
                new TodoItem { Id = Guid.NewGuid(), Description = "Second" });

            // Act
            var result = await service.GetTodoItemsAsync();

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetTodoItemsAsync_EmptyDatabase_ReturnsEmptyList()
        {
            // Act
            var result = await service.GetTodoItemsAsync();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetTodoItemsAsync_WithSearch_FiltersByDescription()
        {
            // Arrange
            SeedItems(
                new TodoItem { Id = Guid.NewGuid(), Description = "Buy milk" },
                new TodoItem { Id = Guid.NewGuid(), Description = "Buy bread" },
                new TodoItem { Id = Guid.NewGuid(), Description = "Walk the dog" });

            // Act
            var result = await service.GetTodoItemsAsync("buy");

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Select(t => t.Description), Is.All.Contains("Buy"));
        }

        [Test]
        public async Task GetTodoItemsAsync_WhitespaceSearch_ReturnsAllItems()
        {
            // Arrange
            SeedItems(
                new TodoItem { Id = Guid.NewGuid(), Description = "First" },
                new TodoItem { Id = Guid.NewGuid(), Description = "Second" });

            // Act
            var result = await service.GetTodoItemsAsync("   ");

            // Assert
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetTodoItemByIdAsync_ExistingId_ReturnsItem()
        {
            // Arrange
            var id = Guid.NewGuid();
            SeedItems(new TodoItem { Id = id, Description = "Target", IsCompleted = false });

            // Act
            var result = await service.GetTodoItemByIdAsync(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(id));
            Assert.That(result.Description, Is.EqualTo("Target"));
        }

        [Test]
        public async Task GetTodoItemByIdAsync_UnknownId_ReturnsNull()
        {
            // Act
            var result = await service.GetTodoItemByIdAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CompleteTodoItemAsync_ExistingIncompleteItem_MarksCompletedAndReturnsDto()
        {
            // Arrange
            var id = Guid.NewGuid();
            SeedItems(new TodoItem { Id = id, Description = "Task", IsCompleted = false });

            // Act
            var result = await service.CompleteTodoItemAsync(id);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsCompleted, Is.True);
            Assert.That(result.CompletedDate, Is.Not.Null);

            var persisted = dbContext.TodoItems.Single(t => t.Id == id);
            Assert.That(persisted.IsCompleted, Is.True);
            Assert.That(persisted.CompletedDate, Is.Not.Null);
        }

        [Test]
        public async Task CompleteTodoItemAsync_UnknownId_ReturnsNull()
        {
            // Act
            var result = await service.CompleteTodoItemAsync(Guid.NewGuid());

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CompleteTodoItemAsync_AlreadyCompleted_ThrowsBusinessLogicException()
        {
            // Arrange
            var id = Guid.NewGuid();
            SeedItems(new TodoItem
            {
                Id = id,
                Description = "Task",
                IsCompleted = true,
                CompletedDate = DateTime.UtcNow.AddDays(-1)
            });

            // Act & Assert
            Assert.That(async () => await service.CompleteTodoItemAsync(id),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The todo item is already completed."));
        }
    }
}
