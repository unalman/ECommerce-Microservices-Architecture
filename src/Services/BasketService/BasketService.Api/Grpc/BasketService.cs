using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain;
using BasketService.Api.Extensions;
using ECommerce.BasketService.Api.Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;
using BasketItem = ECommerce.BasketService.Api.Grpc.BasketItem;

namespace BasketService.Api.Grpc
{
    public class BasketService(IBasketRepository repository,
        ILogger<BasketService> logger) : Basket.BasketBase
    {
        [AllowAnonymous]
        public override async Task<CustomerBasketResponse> GetBasket(GetBasketRequest request, ServerCallContext context)
        {
            var userId = context.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                return new();
            }
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Begin GetBasketById call from method {Medhod} for basket id {Id}", context.Method, userId);
            }
            var data = await repository.GetBasketAsync(userId);
            if (data is not null)
            {
                return MapToCustomerBasketResponse(data);
            }
            return new();
        }

        public override async Task<CustomerBasketResponse> UpdateBasket(UpdateBasketRequest request, ServerCallContext context)
        {
            var userId = context.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                ThrowNotAuthentication();
            }
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Begin UpdateBasket call from method {Medhod} for basket id {Id}", context.Method, userId);
            }
            var customerBasket = MapToCustomerBasket(userId, request);
            var response = await repository.UpdateBasketAsync(customerBasket);
            if (repository is null)
            {
                ThrowBasketDoesNotExist(userId);
            }
            return MapToCustomerBasketResponse(customerBasket);
        }

        public override async Task<DeleteBasketResponse> DeleteBasket(DeleteBasketRequest request, ServerCallContext context)
        {
            var userId = context.GetUserIdentity();
            if (string.IsNullOrEmpty(userId))
            {
                ThrowNotAuthentication();
            }
            await repository.DeleteBasketAsync(userId);
            return new();
        }

        [DoesNotReturn]
        private static void ThrowNotAuthentication() => throw new RpcException(new Status(StatusCode.Unauthenticated, "The caller is not authenticated."));
        [DoesNotReturn]
        private static void ThrowBasketDoesNotExist(string userId) => throw new RpcException(new Status(StatusCode.NotFound, $"Basket with buyer id {userId} does not exist"));

        private static CustomerBasketResponse MapToCustomerBasketResponse(CustomerBasket customerBasket)
        {
            var response = new CustomerBasketResponse();
            foreach (var item in customerBasket.Items)
            {
                response.Items.Add(new BasketItem()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
            }
            return response;
        }
        private static CustomerBasket MapToCustomerBasket(string userId, UpdateBasketRequest customerBasketRequest)
        {
            var response = new CustomerBasket()
            {
                BuyerId = userId
            };
            foreach (var item in customerBasketRequest.Items)
            {
                response.Items.Add(new()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                });
            }
            return response;
        }
    }
}
