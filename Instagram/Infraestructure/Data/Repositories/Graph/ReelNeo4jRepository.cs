using Instagram.Domain.Entities.Media;
using Instagram.Domain.Repositories.Interfaces.Graph.Reel;
using Instagram.Infraestructure.Persistence.Context;
using Neo4j.Driver;
using System.Text;

namespace Instagram.Infraestructure.Data.Repositories.Graph
{
    public class ReelNeo4jRepository : IReelNeo4jRepository
    {
        private readonly Neo4jContext _neo4JContext;
        private readonly ILogger<ReelNeo4jRepository> _logger;
        public ReelNeo4jRepository(ILogger<ReelNeo4jRepository> logger,Neo4jContext neo4JContext)
        {
            _logger = logger;
            _neo4JContext = neo4JContext;
        }

        public async Task AddLikeAsync(string reelId, string userId)
        {
            var query = @"
                MATCH (u:User {Id: $userId}), (r:Reel {reelId: $reelId})
                MERGE (u)-[s:LIKED]->(r)
                SET s.timestamp = timestamp()
                RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "reelId", reelId }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);

            }

            _logger.LogInformation($"A like has been added to reel with ID {reelId} by user with ID {userId}");
        }

        public async Task<bool> CreateReelNodeAsync(ReelEntity reelEntity)
        {
            const int maxRetryAttempts = 3;
            int currentRetry = 0;

            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = BuildQueryReel(reelEntity);

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    var queryResult = await session.RunAsync(query);
                    await queryResult.ForEachAsync(record =>
                    {
                        _logger.LogInformation("Reel Node created with ID: " + record[0].As<Neo4j.Driver.INode>().ElementId);
                    });

                    // Éxito, sal del bucle de reintentos
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating node reel (attempts remaining: {maxRetryAttempts - currentRetry}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5)); // Espera antes de reintentar
                    }
                    else
                    {
                        _logger.LogError("Failed to create node reel after multiple attempts.");
                    }
                }
            }

            return false; // Si llega aquí, significa que todos los reintentos fallaron
        }

        private static string BuildQueryReel(ReelEntity reel)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine("MATCH (u:User)");
            queryBuilder.AppendLine("WHERE u.Id = $userId");
            queryBuilder.AppendLine("CREATE (u)-[:POSTED]->(r:Reel {");

            var parameters = new Dictionary<string, object>
            {
                { "userId", reel.UserId },
                { "reelId", reel.ReelId } // Agrega el campo postid aquí
            };

            if (reel.Tags is not null)
            {
                queryBuilder.AppendLine("tags: $tags,");
                parameters.Add("tags", reel.Tags);
            }

            if (reel.Song?.Artist is not null)
            {
                queryBuilder.AppendLine("artist: $artist,");
                parameters.Add("artist", reel.Song.Artist);
            }

            queryBuilder.AppendLine("}) RETURN p");

            return queryBuilder.ToString();
        }
        public async Task DeleteLikeAsync(string reelId, string userId)
        {
            string query = @"
            MATCH (u:User)-[r:LIKED]->(r:Reel)
            WHERE u.Id = $userId AND r.reelId = $reelId
            DELETE r
            RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "reelId", reelId }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);

            }

            _logger.LogInformation($"A like has been deleted from reel with ID {reelId} by user with ID {userId}");
        }

        public async Task DeleteReelNodeAsync(string userId, string reelId)
        {
            string query = $"MATCH (u:User)-[s:POSTED]->(r:Reel) " +
             "WHERE u.Id = $userId AND r.reelId = $reelId " +
             "DELETE s, p " +
             "RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "reelId", reelId }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);
                await session.CloseAsync();
            }

            _logger.LogInformation($"It has deleted reel {reelId} from user with id {userId}");
        }

        public async Task EditTagsAsync(string userId, string reelId, List<string> tags)
        {
            string query = @"
            MATCH (u:User {Id: $userId})-[:POSTED]->(r:Reel {reelId: $reelId})
            SET p.tags = $newTags
            RETURN p";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "reelId", reelId },
                { "newTags", tags }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);

            }

            _logger.LogInformation($"Tags have been updated for reel with ID {reelId} by user with ID {userId}");
        }
    }
}
