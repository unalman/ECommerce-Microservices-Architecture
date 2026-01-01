using EventBus.Base.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace IntegrationEventLogEF.Services
{
    public class IntegrationEventLogService<TContext> : IIntegrationEventLogService, IDisposable
        where TContext : DbContext
    {
        private volatile bool _disposableValue;
        private readonly TContext _context;
        private readonly Type[] _eventTypes;

        public IntegrationEventLogService(TContext context)
        {
            _context = context;
            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(x => x.Name.EndsWith(nameof(IntegrationEvent)))
                .ToArray();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var result = await _context.Set<IntegrationEventLogEntry>()
                .Where(x => x.TransactionId == transactionId && x.State == EventStateEnum.NotPublihed)
                .ToListAsync();
            if (result.Count != 0)
            {
                return result.OrderBy(x => x.CreationTime)
                    .Select(x => x.DeserializeJsonContent(_eventTypes.FirstOrDefault(y => y.Name == x.EventTypeShoryName)));
            }
            return [];
        }

        public Task SaveEventAsync(IntegrationEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

            _context.Database.UseTransaction(transaction.GetDbTransaction());
            _context.Set<IntegrationEventLogEntry>().Add(eventLogEntry);

            return _context.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _context.Set<IntegrationEventLogEntry>().Single(x => x.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            return _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposableValue)
            {
                if (disposing)
                    _context.Dispose();

                _disposableValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
