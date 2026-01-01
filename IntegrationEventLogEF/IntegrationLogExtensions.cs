
using Microsoft.EntityFrameworkCore;

namespace IntegrationEventLogEF
{
    public static class IntegrationLogExtensions
    {
        public static void UseIntegraitonEventLogs(this ModelBuilder builder)
        {
            builder.Entity<IntegrationEventLogEntry>(builder =>
            {
                builder.ToTable("IntegrationEventLog");

                builder.HasKey(e => e.EventId);
            });
        }
    }
}
