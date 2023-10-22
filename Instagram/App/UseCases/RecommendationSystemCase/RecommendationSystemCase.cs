using Instagram.Domain.Repositories.Interfaces.Graph.User;

namespace Instagram.App.UseCases.RecommendationSystemCase
{
    public class RecommendationSystemCase : IRecommendationSystemCase
    {
        private readonly IUserNeo4jRepository _userRepository;
        public RecommendationSystemCase(IUserNeo4jRepository userRepository)
        {
            _userRepository = userRepository;     
        }

    }

}
