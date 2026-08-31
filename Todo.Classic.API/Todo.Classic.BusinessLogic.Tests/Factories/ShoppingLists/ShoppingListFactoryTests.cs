using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Tests.Factories.ShoppingLists
{
    [TestFixture]
    public class ShoppingListFactoryTests
    {
        private ShoppingListFactory factory;

        [SetUp]
        public void SetUp()
        {
            factory = new ShoppingListFactory();
        }

        [Test]
        public void CreateShoppingList_ValidRequest_ReturnsShoppingList()
        {
            var request = new CreateShoppingListRequest("Groceries");

            var result = factory.CreateShoppingList(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(result.Title, Is.EqualTo("Groceries"));
        }

        [Test]
        public void CreateShoppingList_NullRequest_ThrowsArgumentNullException()
        {
            Assert.That(() => factory.CreateShoppingList(null!), Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void CreateShoppingList_NullOrWhitespaceTitle_ThrowsBusinessLogicException(string? title)
        {
            var request = new CreateShoppingListRequest(title!);

            Assert.That(() => factory.CreateShoppingList(request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The title cannot be null or empty."));
        }

        [Test]
        public void CreateShoppingList_TitleExceedsMaxLength_ThrowsBusinessLogicException()
        {
            var request = new CreateShoppingListRequest(new string('a', 201));

            Assert.That(() => factory.CreateShoppingList(request),
                Throws.TypeOf<BusinessLogicException>()
                    .With.Message.EqualTo("The title cannot exceed 200 characters."));
        }
    }
}
