
using EventBus.Base.Events;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace IntegrationEventLogEF
{
    public class IntegrationEventLogEntry
    {
        private static readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };
        private static readonly JsonSerializerOptions s_caseIntensiveOptions = new() { PropertyNameCaseInsensitive = true };

        private IntegrationEventLogEntry() { }
        public IntegrationEventLogEntry(IntegrationEvent @event, Guid transactionId)
        {
            EventId = @event.Id;
            CreationTime = @event.CreateDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonSerializer.Serialize(@event, @event.GetType(), s_indentedOptions);
            State = EventStateEnum.NotPublihed;
            TimesSent = 0;
            TransactionId = transactionId;
        }
        public Guid EventId { get; private set; }

        [Required]
        public string EventTypeName { get; private set; }

        [NotMapped]
        public string EventTypeShoryName => EventTypeName.Split('.').Last();

        [NotMapped]
        public IntegrationEvent IntegrationEvent { get; private set; }

        public EventStateEnum State { get; set; }

        public int TimesSent { get; set; }

        public DateTime CreationTime { get; private set; }

        [Required]
        public string Content { get; private set; }

        public Guid TransactionId { get; private set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonSerializer.Deserialize(Content, type, s_caseIntensiveOptions) as IntegrationEvent;
            return this;
        }
    }
}
