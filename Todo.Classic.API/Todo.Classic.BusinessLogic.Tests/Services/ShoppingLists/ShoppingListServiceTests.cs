using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Todo.Classic.BusinessLogic.Factories.ShoppingLists;
using Todo.Classic.BusinessLogic.Services.ShoppingLists;
using Todo.Classic.DataAccess.Context;
using Todo.Classic.Helpers.BusinessLogic;
using Todo.Classic.Model.Domain.ShoppingLists;
using Todo.Classic.Model.DTO.ShoppingLists;

namespace Todo.Classic.BusinessLogic.Tests.Services.ShoppingLists
{
    [TestFixture]
    public class ShoppingListServiceTests
    {
        private IShoppingListFactory factory;
        private TodoDbContext dbContext;
        private ILogger<ShoppingListService> logger;
        private ShoppingListService service;

        [SetUp]
        public void SetUp()
        {
            factory = Substitute.For<IShoppingListFactory>();
            logger = Substitute.For<ILogger<ShoppingListService>>();

            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            dbContext = new TodoDbContext(options);

            service = new ShoppingListService(factory, dbContext, logger);
        }

        [TearDown]
        public void TearDown()
        {
            dbContext.Dispose();
        }

        private ShoppingList SeedList(string title = "List", params ShoppingListItem[] items)
        {
            var list = new ShoppingList { Id = Guid.NewGuid(), Title = title };
            dbContext.ShoppingLists.Add(list);
            foreach (var i in items)
            {
                i.ShoppingListId = list.Id;
                dbContext.ShoppingListItems.Add(i);
            }
            dbContext.SaveChanges();
            return list;
        }

        [Test]
        public async Task CreateShoppingListAsync_ValidRequest_AddsAndReturnsId()
        {
            var request = new CreateShoppingListRequest("Groceries");
            var entity = new ShoppingList { Id = Guid.NewGuid(), Title = "Groceries" };
            factory.CreateShoppingList(request).Returns(entity);

            var result = await service.CreateShoppingListAsync(request);

            Assert.That(result, Is.EqualTo(entity.Id));
            Assert.That(dbContext.ShoppingLists.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateShoppingListAsync_FactoryThrows_Propagates()
        {
            var request = new CreateShoppingListRequest("bad");
            factory.CreateShoppingList(request).Throws(new BusinessLogicException("invalid"));

            Assert.That(async () => await service.CreateShoppingListAsync(request),
                Throws.TypeOf<BusinessLogicException>());
            Assert.That(dbContext.ShoppingLists.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetShoppingListsAsync_NoSearch_ReturnsAll()
        {
            SeedList("First");
            SeedList("Second");

            var result = await service.GetShoppingListsAsync();

            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetShoppingListsAsync_WithSearch_FiltersByTitle()
        {
            SeedList("Groceries");
            SeedList("Hardware");

            var result = await service.GetShoppingListsAsync("groc");

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Groceries"));
        }

        [Test]
        public async Task GetShoppingListByIdAsync_Unknown_ReturnsNull()
        {
            var result = await service.GetShoppingListByIdAsync(Guid.NewGuid());
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateShoppingListAsync_Existing_UpdatesTitle()
        {
            var list = SeedList("Old");

            var result = await service.UpdateShoppingListAsync(list.Id, new UpdateShoppingListRequest("New"));

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("New"));
            Assert.That(dbContext.ShoppingLists.Single(l => l.Id == list.Id).Title, Is.EqualTo("New"));
        }

        [Test]
        public async Task UpdateShoppingListAsync_Unknown_ReturnsNull()
        {
            var result = await service.UpdateShoppingListAsync(Guid.NewGuid(), new UpdateShoppingListRequest("x"));
            Assert.That(result, Is.Null);
        }

        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void UpdateShoppingListAsync_InvalidTitle_ThrowsBusinessLogicException(string? title)
        {
            var list = SeedList("Old");

            Assert.That(async () => await service.UpdateShoppingListAsync(list.Id, new UpdateShoppingListRequest(title!)),
                Throws.TypeOf<BusinessLogicException>());
        }

        [Test]
        public async Task DeleteShoppingListAsync_Existing_RemovesListAndCascadesItems()
        {
            var list = SeedList("Groceries",
                new ShoppingListItem { Id = Guid.NewGuid(), Title = "Milk" },
                new ShoppingListItem { Id = Guid.NewGuid(), Title = "Bread" });

            var result = await service.DeleteShoppingListAsync(list.Id);

            Assert.That(result, Is.True);
            Assert.That(dbContext.ShoppingLists.Count(), Is.EqualTo(0));
            Assert.That(dbContext.ShoppingListItems.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteShoppingListAsync_Unknown_ReturnsFalse()
        {
            var result = await service.DeleteShoppingListAsync(Guid.NewGuid());
            Assert.That(result, Is.False);
        }
    }
}
