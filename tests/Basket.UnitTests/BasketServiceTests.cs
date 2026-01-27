using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain;
using ECommerce.Basket.UnitTests.Helpers;
using ECommerce.BasketService.Api.Grpc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BasketItem = BasketService.Api.Core.Domain.BasketItem;
using BasketServiceBase = BasketService.Api.Grpc.BasketService;

namespace ECommerce.Basket.UnitTests;

[TestClass]
public class BasketServiceTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task GetBasketReturnsEmptyForNoUser()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        var service = new BasketServiceBase(mockRepository, NullLogger<BasketServiceBase>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        serverCallContext.SetUserState("__HttpContext", new DefaultHttpContext());

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.IsEmpty(response.Items);
    }

    [TestMethod]
    public async Task GetBasketReturnsItemsForValidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketServiceBase(mockRepository, NullLogger<BasketServiceBase>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "1")]));
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.HasCount(1, response.Items);
    }

    [TestMethod]
    public async Task GetBasketReturnsEmptyForInvalidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketServiceBase(mockRepository, NullLogger<BasketServiceBase>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        var httpContext = new DefaultHttpContext();
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.IsEmpty(response.Items);
    }
}
