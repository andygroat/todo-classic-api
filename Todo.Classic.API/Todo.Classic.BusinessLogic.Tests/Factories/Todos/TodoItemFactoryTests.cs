using Todo.Classic.BusinessLogic.Factories.Todos;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.Todos;

namespace Todo.Classic.BusinessLogic.Tests.Factories.Todos
{
    [TestFixture]
    public class TodoItemFactoryTests
    {
        private TodoItemFactory factory;

        /// <summary>
        /// Sets up the test environment before each test is run.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Create a new instance of the factory before each test
            factory = new TodoItemFactory();
        }

        [Test]
        public void CreateTodoItem_ValidRequest_ReturnsTodoItem()
        {
            // Arrange
            var request = new CreateTodoRequest("A valid description", DateTime.UtcNow.AddDays(1));

            // Act
            var result = factory.CreateTodoItem(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Description, Is.EqualTo(request.Description));
            Assert.That(result.DueDate, Is.EqualTo(request.DueDate));
            Assert.That(result.IsCompleted, Is.False);
        }

        [Test]
        public void CreateTodoItem_ValidRequestWithoutDueDate_ReturnsTodoItem()
        {
            // Arrange
            var request = new CreateTodoRequest("A valid description", null);

            // Act
            var result = factory.CreateTodoItem(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.DueDate, Is.Null);
            Assert.That(result.IsCompleted, Is.False);
        }

        [Test]
        public void CreateTodoItem_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.That(() => factory.CreateTodoItem(null!), Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateTodoItem_NullOrWhitespaceDescription_ThrowsBusinessLogicException(string? description)
        {
            // Arrange
            var request = new CreateTodoRequest(description!, null);

            // Act & Assert
            Assert.That(() => factory.CreateTodoItem(request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The description cannot be null or empty."));
        }

        [Test]
        public void CreateTodoItem_DescriptionExceedsMaxLength_ThrowsBusinessLogicException()
        {
            // Arrange
            var request = new CreateTodoRequest(new string('a', 101), null);

            // Act & Assert
            Assert.That(() => factory.CreateTodoItem(request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The description cannot exceed 100 characters."));
        }

        [Test]
        public void CreateTodoItem_DueDateInPast_ThrowsBusinessLogicException()
        {
            // Arrange
            var request = new CreateTodoRequest("A valid description", DateTime.UtcNow.AddDays(-1));

            // Act & Assert
            Assert.That(() => factory.CreateTodoItem(request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The due date cannot be in the past."));
        }
    }
}
