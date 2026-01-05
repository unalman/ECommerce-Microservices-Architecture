using CatalogService.Api.Infrastructure.Context;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using IntegrationEventLogEF.Services;
using IntegrationEventLogEF.Utilities;

namespace CatalogService.Api.IntegrationEvents
{
    public class CatalogIntegrationEventService(ILogger<CatalogIntegrationEventService> logger,
        IEventBus eventBus,
        CatalogContext catalogContext,
        IIntegrationEventLogService integrationEventLogService) : ICatalogIntegrationEventService, IDisposable
    {
        private volatile bool disposedValue;

        public async Task PublishThroughEventBusAsync(IntegrationEvent evt)
        {
            try
            {
                await integrationEventLogService.MarkEventAsInProgressAsync(evt.Id);
                await eventBus.PublishAsync(evt);
                await integrationEventLogService.MarkEventAsPublishedAsync(evt.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error Publish integration event: {IntegrationEventId} - ({@IntegrationEvent})", evt.Id, evt);
                await integrationEventLogService.MarkEventAsFailedAsync(evt.Id);
            }
        }

        public async Task SaveEventAndCatalogContextChangesAsync(IntegrationEvent evt)
        {
            logger.LogInformation("CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationeventId}", evt.Id);

            await ResilientTransaction.New(catalogContext).ExecuteAsync(async () =>
            {
                await catalogContext.SaveChangesAsync();
                await integrationEventLogService.SaveEventAsync(evt, catalogContext.Database.CurrentTransaction);
            });
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                (integrationEventLogService as IDisposable)?.Dispose();
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
