using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Tests.Factories.ShoppingLists
{
    [TestFixture]
    public class ShoppingListItemFactoryTests
    {
        private ShoppingListItemFactory factory;

        [SetUp]
        public void SetUp()
        {
            factory = new ShoppingListItemFactory();
        }

        [Test]
        public void CreateShoppingListItem_ValidRequest_ReturnsItem()
        {
            var listId = Guid.NewGuid();
            var request = new CreateShoppingListItemRequest("Milk");

            var result = factory.CreateShoppingListItem(listId, request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.ShoppingListId, Is.EqualTo(listId));
            Assert.That(result.Title, Is.EqualTo("Milk"));
            Assert.That(result.IsComplete, Is.False);
        }

        [Test]
        public void CreateShoppingListItem_NullRequest_ThrowsArgumentNullException()
        {
            Assert.That(() => factory.CreateShoppingListItem(Guid.NewGuid(), null!),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void CreateShoppingListItem_EmptyShoppingListId_ThrowsBusinessLogicException()
        {
            var request = new CreateShoppingListItemRequest("Milk");

            Assert.That(() => factory.CreateShoppingListItem(Guid.Empty, request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The shopping list id cannot be empty."));
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateShoppingListItem_NullOrWhitespaceTitle_ThrowsBusinessLogicException(string? title)
        {
            var request = new CreateShoppingListItemRequest(title!);

            Assert.That(() => factory.CreateShoppingListItem(Guid.NewGuid(), request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The title cannot be null or empty."));
        }

        [Test]
        public void CreateShoppingListItem_TitleExceedsMaxLength_ThrowsBusinessLogicException()
        {
            var request = new CreateShoppingListItemRequest(new string('a', 201));

            Assert.That(() => factory.CreateShoppingListItem(Guid.NewGuid(), request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The title cannot exceed 200 characters."));
        }
    }
}
