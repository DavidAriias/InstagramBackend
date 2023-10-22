using Instagram.config.constants;
using Microsoft.Extensions.Options;
using Neo4j.Driver;

namespace Instagram.Infraestructure.Persistence.Context
{
    public class Neo4jContext
    {
        private readonly IDriver _driver;
        public Neo4jContext(IOptions<Neo4jConfig> neo4jSettings)
        {
            var config = neo4jSettings.Value;
            _driver = GraphDatabase.Driver(new Uri(config.ServerUrl),
                AuthTokens.Basic(config.Username, config.Password)
            );
            _driver.VerifyConnectivityAsync();
        }

        public IDriver GetDriver => _driver;
    }
}
