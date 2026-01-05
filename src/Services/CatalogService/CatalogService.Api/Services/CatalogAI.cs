using CatalogService.Api.Core.Domain;
using Microsoft.Extensions.AI;
using Pgvector;
using System.Diagnostics;

namespace CatalogService.Api.Services
{
    public class CatalogAI : ICatalogAI
    {
        private const int EmbeddingDimensions = 384;
        private readonly IEmbeddingGenerator<string, Embedding<float>>? _embeddingGenerator;

        /// <summary>The web host environment.</summary>
        private readonly IWebHostEnvironment _environment;
        /// <summary>Logger for use in AI operations.</summary>
        private readonly ILogger _logger;

        public CatalogAI(IWebHostEnvironment environment, ILogger<CatalogAI> logger, IEmbeddingGenerator<string, Embedding<float>>? embeddingGenerator = null)
        {
            _embeddingGenerator = embeddingGenerator;
            _environment = environment;
            _logger = logger;
        }

        public bool IsEnabled => _embeddingGenerator is not null;

        public ValueTask<Vector?> GetEmbeddingAsync(CatalogItem item) =>
       IsEnabled ?
       GetEmbeddingAsync(CatalogItemToString(item))
       : ValueTask.FromResult<Vector>(null);

        public async ValueTask<IReadOnlyList<Vector>?> GetEmbeddingsAsync(IEnumerable<CatalogItem> item)
        {
            if (IsEnabled)
            {
                long timestamp = Stopwatch.GetTimestamp();
                GeneratedEmbeddings<Embedding<float>> embeddings = embeddings = await _embeddingGenerator!.GenerateAsync(item.Select(CatalogItemToString));
                var results = embeddings.Select(x => new Vector(x.Vector[0..EmbeddingDimensions])).ToList();

                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s", results.Count, Stopwatch.GetElapsedTime(timestamp).TotalSeconds);
                }
                return results;
            }
            return null;
        }

        public async ValueTask<Vector?> GetEmbeddingAsync(string text)
        {
            if (IsEnabled)
            {
                long timestamp = Stopwatch.GetTimestamp();

                var embedding = await _embeddingGenerator!.GenerateVectorAsync(text);
                embedding = embedding[0..EmbeddingDimensions];

                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace("Generated embedding in {ElapsedMilliseconds}s: '{Text}'", Stopwatch.GetElapsedTime(timestamp).TotalSeconds, text);
                }

                return new Vector(embedding);
            }
            return null;
        }

        private static string CatalogItemToString(CatalogItem item) => $"{item.Name} {item.Description}";
    }
}
