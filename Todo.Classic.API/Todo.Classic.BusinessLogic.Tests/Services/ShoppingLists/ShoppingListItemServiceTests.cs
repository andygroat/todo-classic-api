using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Tests.Services.ShoppingLists
{
    [TestFixture]
    public class ShoppingListItemServiceTests
    {
        private IShoppingListItemFactory factory;
        private TodoDbContext dbContext;
        private ILogger<ShoppingListItemService> logger;
        private ShoppingListItemService service;

        [SetUp]
        public void SetUp()
        {
            factory = Substitute.For<IShoppingListItemFactory>();
            logger = Substitute.For<ILogger<ShoppingListItemService>>();

            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new TodoDbContext(options);

            service = new ShoppingListItemService(factory, dbContext, logger);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private Guid SeedList()
        {
            var list = new ShoppingList { Id = Guid.NewGuid(), Title = "List" };
            dbContext.ShoppingLists.Add(list);
            dbContext.SaveChanges();
            return list.Id;
        }

        private ShoppingListItem SeedItem(Guid listId, string title = "Item", bool isComplete = false)
        {
            var item = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = listId, Title = title, IsComplete = isComplete };
            dbContext.ShoppingListItems.Add(item);
            dbContext.SaveChanges();
            return item;
        }

        [Test]
        public async Task CreateShoppingListItemAsync_ListExists_AddsAndReturnsId()
        {
            var listId = SeedList();
            var request = new CreateShoppingListItemRequest("Milk");
            var entity = new ShoppingListItem { Id = Guid.NewGuid(), ShoppingListId = listId, Title = "Milk" };
            factory.CreateShoppingListItem(listId, request).Returns(entity);

            var result = await service.CreateShoppingListItemAsync(listId, request);

            Assert.That(result, Is.EqualTo(entity.Id));
            Assert.That(dbContext.ShoppingListItems.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CreateShoppingListItemAsync_ListMissing_ReturnsNull()
        {
            var result = await service.CreateShoppingListItemAsync(Guid.NewGuid(), new CreateShoppingListItemRequest("Milk"));

            Assert.That(result, Is.Null);
            Assert.That(dbContext.ShoppingListItems.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetShoppingListItemsAsync_ListExists_ReturnsItems()
        {
            var listId = SeedList();
            SeedItem(listId, "Milk");
            SeedItem(listId, "Bread");

            var result = await service.GetShoppingListItemsAsync(listId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetShoppingListItemsAsync_ListMissing_ReturnsNull()
        {
            var result = await service.GetShoppingListItemsAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetShoppingListItemByIdAsync_Existing_ReturnsItem()
        {
            var listId = SeedList();
            var item = SeedItem(listId, "Milk");

            var result = await service.GetShoppingListItemByIdAsync(listId, item.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(item.Id));
        }

        [Test]
        public async Task GetShoppingListItemByIdAsync_Unknown_ReturnsNull()
        {
            var listId = SeedList();
            var result = await service.GetShoppingListItemByIdAsync(listId, Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateShoppingListItemAsync_Existing_UpdatesTitle()
        {
            var listId = SeedList();
            var item = SeedItem(listId, "Old");

            var result = await service.UpdateShoppingListItemAsync(listId, item.Id, new UpdateShoppingListItemRequest("New"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("New"));
        }

        [Test]
        public async Task UpdateShoppingListItemAsync_Unknown_ReturnsNull()
        {
            var listId = SeedList();
            var result = await service.UpdateShoppingListItemAsync(listId, Guid.NewGuid(), new UpdateShoppingListItemRequest("x"));
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CompleteShoppingListItemAsync_Existing_MarksComplete()
        {
            var listId = SeedList();
            var item = SeedItem(listId, "Milk", isComplete: false);

            var result = await service.CompleteShoppingListItemAsync(listId, item.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.IsComplete, Is.True);
            Assert.That(dbContext.ShoppingListItems.Single(i => i.Id == item.Id).IsComplete, Is.True);
        }

        [Test]
        public async Task CompleteShoppingListItemAsync_Unknown_ReturnsNull()
        {
            var listId = SeedList();
            var result = await service.CompleteShoppingListItemAsync(listId, Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CompleteShoppingListItemAsync_AlreadyComplete_ThrowsBusinessLogicException()
        {
            var listId = SeedList();
            var item = SeedItem(listId, "Milk", isComplete: true);

            Assert.That(async () => await service.CompleteShoppingListItemAsync(listId, item.Id),
                Throws.TypeOf<BusinessLogicException>());
        }

        [Test]
        public async Task DeleteShoppingListItemAsync_Existing_RemovesItem()
        {
            var listId = SeedList();
            var item = SeedItem(listId, "Milk");

            var result = await service.DeleteShoppingListItemAsync(listId, item.Id);

            Assert.That(result, Is.True);
            Assert.That(dbContext.ShoppingListItems.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteShoppingListItemAsync_Unknown_ReturnsFalse()
        {
            var listId = SeedList();
            var result = await service.DeleteShoppingListItemAsync(listId, Guid.NewGuid());
            Assert.That(result, Is.False);
        }
    }
}
