using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using OrderService.Api.Apis;
using OrderService.Api.Application.Commands;
using OrderService.Api.Application.Queries;
using OrderService.Api.Infrastructure.Services;

namespace Ordering.UnitTests.Application
{
    [TestClass]
    public class OrdersWebApiTest
    {
        private readonly IMediator _mediatorMock;
        private readonly IOrderQueries _orderQueriesMock;
        private readonly IIdentityService _identityServiceMock;
        private readonly ILogger<OrderServices> _loggerMock;

        public OrdersWebApiTest()
        {
            _mediatorMock = Substitute.For<IMediator>();
            _orderQueriesMock = Substitute.For<IOrderQueries>();
            _identityServiceMock = Substitute.For<IIdentityService>();
            _loggerMock = Substitute.For<ILogger<OrderServices>>();
        }

        [TestMethod]
        public async Task Cancel_order_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Send(Arg.Any<IdentifiedCommand<CancelOrderCommand, bool>>(), default)
                .Returns(Task.FromResult(true));

            //act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.CancelOrderAsync(Guid.NewGuid(), new CancelOrderCommand(1), orderServices);

            //Assert
            Assert.IsInstanceOfType<Ok>(result.Result);
        }

        [TestMethod]
        public async Task Cancel_order_bad_request()
        {
            //Arrange
            _mediatorMock.Send(Arg.Any<IdentifiedCommand<CancelOrderCommand, bool>>(), default)
                .Returns(Task.FromResult(true));

            //Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.CancelOrderAsync(Guid.Empty, new CancelOrderCommand(1), orderServices);

            //Assert
            Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
        }

        [TestMethod]
        public async Task Ship_order_with_requestId_success()
        {
            //Arrange
            _mediatorMock.Send(Arg.Any<IdentifiedCommand<ShipOrderCommand, bool>>(), default)
                 .Returns(Task.FromResult(true));

            //Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.ShipOrderAsync(Guid.NewGuid(), new ShipOrderCommand(1), orderServices);

            //Assert
            Assert.IsInstanceOfType<Ok>(result.Result);
        }

        [TestMethod]
        public async Task Ship_order_bad_request()
        {
            //Arrange
            _mediatorMock.Send(Arg.Any<IdentifiedCommand<CreateOrderCommand, bool>>(), default)
                .Returns(Task.FromResult(true));

            //Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.ShipOrderAsync(Guid.Empty, new ShipOrderCommand(1), orderServices);

            //Assert
            Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
        }

        [TestMethod]
        public async Task Get_orders_success()
        {
            //Arrange
            var fakeDynamicResult = Enumerable.Empty<OrderSummary>();

            _identityServiceMock.GetUserIdentity()
                .Returns(Guid.NewGuid().ToString());
            _orderQueriesMock.GetOrdersFromUserAsync(Guid.NewGuid().ToString())
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.GetOrdersByUserAsync(orderServices);

            //Assert
            Assert.IsInstanceOfType<Ok<IEnumerable<OrderSummary>>>(result);
        }
        [TestMethod]
        public async Task Get_order_success()
        {
            //Arrange
            var fakeOrderId = 123;
            var fakeDynamicResult = new Order();
            _orderQueriesMock.GetOrderAsync(Arg.Any<int>())
                .Returns(Task.FromResult(fakeDynamicResult));

            //Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.GetOrderAsync(fakeOrderId, orderServices);

            //Assert
            Assert.IsInstanceOfType<Ok<Order>>(result.Result);
            Assert.AreSame(fakeDynamicResult, ((Ok<Order>)result.Result).Value);
        }
        [TestMethod]
        public async Task Get_order_fails()
        {
            // Arrange
            var fakeOrderId = 123;
#pragma warning disable NS5003
            _orderQueriesMock.GetOrderAsync(Arg.Any<int>())
                .Throws(new KeyNotFoundException());
#pragma warning restore NS5003

            // Act
            var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
            var result = await OrderApi.GetOrderAsync(fakeOrderId, orderServices);

            // Assert
            Assert.IsInstanceOfType<NotFound>(result.Result);
        }

        [TestMethod]
        public async Task Get_cartTypes_success()
        {
            //arrange
            var fakeDynamicResult = Enumerable.Empty<CardType>();
            _orderQueriesMock.GetCardTypesAsync()
                .Returns(Task.FromResult(fakeDynamicResult));

            //act
            var result = await OrderApi.GetCardTypesAsync(_orderQueriesMock);

            //Assert
            Assert.IsInstanceOfType<Ok<IEnumerable<CardType>>>(result);
            Assert.AreSame(fakeDynamicResult, result.Value);
        }
    }
}
