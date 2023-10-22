using Instagram.config.constants;
using Instagram.Domain.Entities.Media;
using Instagram.Domain.Entities.User;
using Instagram.Domain.Repositories.Interfaces.Graph.User;
using Instagram.Infraestructure.Persistence.Context;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Instagram.Infraestructure.Data.Repositories.Graph
{
    public class UserNeo4jRepository : IUserNeo4jRepository
    {
        private readonly Neo4jContext _neo4JContext;
        public UserNeo4jRepository(Neo4jContext neo4JContext)
        {
            _neo4JContext = neo4JContext;

        }

        public async Task CreateUser(UserEntity user)
        {

            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = "CREATE (u:User {Id: $id, Username: $username})";
            var parameters = new { id = user.Id.ToString(), username = user.Username };

            await session.RunAsync(query, parameters);
            await session.CloseAsync();
        }

        public async Task<IEnumerable<string>> FollowersFromUser(string userId)
        {
            List<string> followersIds = new();

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                var query = $@"MATCH (u:User {{Id:'{userId}'}})<-[:FOLLOW_TO]-(followed) RETURN followed";

                var result = await session.RunAsync(query);

                await result.ForEachAsync(record =>
                {
                    // Accede a los valores de las propiedades del registro
                    var followedNode = record["followed"].As<Neo4j.Driver.INode>();

                    // Puedes acceder a las propiedades del nodo "followed" aquí según sea necesario
                    var follower = followedNode["Id"].As<string>();

                    followersIds.Add(follower);
                });

                await session.CloseAsync();
            }

            return followersIds;
        }

        public async Task FollowToUser(string followerId, string userId)
        {
            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = $@"
                MATCH (follower:User {{Id:'{followerId}'}}), (followed:User {{Id:'{userId}'}})
                CREATE (follower)-[:FOLLOW_TO {{dateFollow:datetime()}}]->(followed)";

            await session.RunAsync(query);
            await session.CloseAsync();
        }

        public async Task UnfollowUser(string followerId, string userId)
        {
            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = $@"MATCH (follower:User {{Id:'{followerId}'}})-[rel:FOLLOW_TO]->(followed:User {{Id:'{userId}'}})
                        DELETE rel";

            await session.RunAsync(query);
            await session.CloseAsync();


        }

        public async Task UpdateUsername(string username, string userId)
        {
            using var session = _neo4JContext.GetDriver.AsyncSession();
            string query = $"MATCH (u:User) WHERE u.Id = '{userId}' SET u.Username = '{username}' " +
                $"RETURN u";

            await session.RunAsync(query);
            await session.CloseAsync();
        }

        public async Task<bool> IsUserFollowToOther(string followerId, string userId)
        {
            bool isFollowed = false;

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                string query = @$"MATCH (follower:User {{Id:'{followerId}'}})-[:FOLLOW_TO]->(followed:User {{Id:'{userId}'}}) 
                            RETURN COUNT(followed) > 0 AS FOLLOW_TO";

                var result = await session.RunAsync(query);
                var record = await result.SingleAsync();

                isFollowed = record["FOLLOW_TO"].As<bool>();

                await session.CloseAsync();
            }

            return isFollowed;
        }

        public async Task<IEnumerable<string>> UsersFollowedByOthers(string userId)
        {
            List<string> followersIds = new();

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {
                string query = $@"MATCH (u:User {{Id:'{userId}'}})-[:FOLLOW_TO]->(followed) RETURN followed";

                var result = await session.RunAsync(query);

                await result.ForEachAsync(record =>
                {
                    // Accede a los valores de las propiedades del registro
                    var followedNode = record["followed"].As<Neo4j.Driver.INode>();

                    // Puedes acceder a las propiedades del nodo "followed" aquí según sea necesario
                    var follower = followedNode["Id"].As<string>();

                    followersIds.Add(follower);
                });

                await session.CloseAsync();
            }

            return followersIds;
        }

        public async Task<IEnumerable<string>> SuggestionFollowers(string userId)
        {
            List<string> suggestionsIds = new();

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {

                string query = $@"
                    MATCH (u:User {{Id:'{userId}'}})-[:FOLLOW_TO]->(followers),
                    (followers)-[:FOLLOW_TO]->(suggestions)
                    WHERE NOT (u)-[:FOLLOW_TO]->(suggestions)
                    RETURN suggestions.Id AS suggestFollower
                    LIMIT 10
                ";

                var result = await session.RunAsync(query);

                await result.ForEachAsync(record =>
                {

                    // Puedes acceder a las propiedades del nodo "followed" aquí según sea necesario
                    string suggestionId = record["suggestFollower"].As<string>();

                    suggestionsIds.Add(suggestionId);
                });

                await session.CloseAsync();
            }

            return suggestionsIds;
        }

        public Task<IEnumerable<PostEntity>> SuggestionPosts(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReelEntity>> SuggestionReels(string userId)
        {
            List<ReelEntity> suggestionsIds = new();

            using (var session = _neo4JContext.GetDriver.AsyncSession())
            {

                string query = $@"
                    MATCH (u:User {{Id:'{userId}'}})-[:FOLLOW_TO]->(followers),
                    (followers)-[:FOLLOW_TO]->(suggestions)
                    WHERE NOT (u)-[:FOLLOW_TO]->(suggestions)
                    RETURN suggestions.Id AS suggestFollower
                    LIMIT 10
                ";

                var result = await session.RunAsync(query);

                await result.ForEachAsync(record =>
                {

                    // Puedes acceder a las propiedades del nodo "followed" aquí según sea necesario
                    string suggestionId = record["suggestFollower"].As<string>();

                    //suggestionsIds.Add(suggestionId);
                });

                await session.CloseAsync();
            }

            return suggestionsIds;
        }
    }
}
