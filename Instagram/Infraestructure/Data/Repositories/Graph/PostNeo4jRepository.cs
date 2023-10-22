using Instagram.Domain.Entities.Media;
using Instagram.Domain.Repositories.Interfaces.Graph.Post;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;
using System.Text;
using INode = Neo4j.Driver.INode;

namespace Instagram.Infraestructure.Data.Repositories.Graph
{
    public class PostNeo4jRepository : IPostNeo4jRepository
    {
        private readonly Neo4jContext _neo4JContext;
        private readonly ILogger<PostNeo4jRepository> _logger;
        public PostNeo4jRepository(
            Neo4jContext neo4JContext,
            ILogger<PostNeo4jRepository> logger
            ) 
        {
            _neo4JContext = neo4JContext;
            _logger = logger;
        }
        public async Task AddLikeAsync(string postId, string userId)
        {
            var query = @"
                MATCH (u:User {Id: $userId}), (p:Post {postId: $postId})
                MERGE (u)-[r:LIKED]->(p)
                SET r.timestamp = timestamp()
                RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "postId", postId }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);

            }

            _logger.LogInformation($"A like has been added to post with ID {postId} by user with ID {userId}");
        }
        public async Task DeleteLikeAsync(string postId, string userId)
        {
            string query = @"
            MATCH (u:User)-[r:LIKED]->(p:Post)
            WHERE u.Id = $userId AND p.postId = $postId
            DELETE r
            RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "postId", postId }
            };

           using (var session = _neo4JContext.GetDriver.AsyncSession())
           {
             await session.RunAsync(query, parameters);

           }

            _logger.LogInformation($"A like has been deleted from post with ID {postId} by user with ID {userId}");
        }

        public async Task EditTagsAsync(string userId, string postId, List<string> tags)
        {
            string query = @"
            MATCH (u:User {Id: $userId})-[:POSTED]->(p:Post {postId: $postId})
            SET p.tags = $newTags
            RETURN p";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "postId", postId },
                { "newTags", tags }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query, parameters);

            }

            _logger.LogInformation($"Tags have been updated for post with ID {postId} by user with ID {userId}");
        }

        public async Task<bool> CreatePostNodeAsync(PostEntity post)
        {
            const int maxRetryAttempts = 3;
            int currentRetry = 0;

            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = BuildQueryPost(post);

            while (currentRetry < maxRetryAttempts)
            {
                try
                {
                    var queryResult = await session.RunAsync(query);
                    await queryResult.ForEachAsync(record =>
                    {
                        _logger.LogInformation("Post Node created with ID: " + record[0].As<INode>().ElementId);
                    });

                    // Éxito, sal del bucle de reintentos
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating node post (attempts remaining: {maxRetryAttempts - currentRetry}): {ex.Message}");
                    currentRetry++;

                    if (currentRetry < maxRetryAttempts)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(5)); // Espera antes de reintentar
                    }
                    else
                    {
                        _logger.LogError("Failed to create node post after multiple attempts.");
                    }
                }
            }

            return false; // Si llega aquí, significa que todos los reintentos fallaron
        }

        private static string BuildQueryPost(PostEntity post)
        {
            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine("MATCH (u:User)");
            queryBuilder.AppendLine("WHERE u.Id = $userId");
            queryBuilder.AppendLine("CREATE (u)-[:POSTED]->(p:Post {");

            var parameters = new Dictionary<string, object>
            {
                { "userId", post.UserId },
                { "postId", post.PostId } // Agrega el campo postid aquí
            };

            if (post.Hashtags is not null)
            {
                queryBuilder.AppendLine("tags: $tags,");
                parameters.Add("tags", post.Hashtags);
            }

            if (post.Song?.Artist is not null)
            {
                queryBuilder.AppendLine("artist: $artist,");
                parameters.Add("artist", post.Song.Artist);
            }

            queryBuilder.AppendLine("}) RETURN p");

            return queryBuilder.ToString();
        }

        public async Task DeletePostNodeAsync(string userId, string postId)
        {
            string query = $"MATCH (u:User)-[r:POSTED]->(p:Post) " +
                "WHERE u.Id = $userId AND p.postId = $postId " +
                "DELETE r, p " +
                "RETURN true AS success";

            var parameters = new Dictionary<string, object>
            {
                { "userId", userId },
                { "postId", postId }
            };

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                await session.RunAsync(query,parameters);
                await session.CloseAsync();
            }

            _logger.LogInformation($"It has deleted post {postId} from user with id {userId}");
        }
    }
}
