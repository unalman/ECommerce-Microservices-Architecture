
namespace OrderProcessor.Extensions
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
          

            builder.AddNpgsqlDataSource("orderingdb");

            builder.Services.AddOptions<BackgroundTaskOptions>()
                .BindConfiguration(nameof(BackgroundTaskOptions));
        }
    }
}
