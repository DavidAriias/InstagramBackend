using HotChocolate.Authorization;
using Instagram.App.UseCases.MediaCases.PostCase;
using Instagram.App.UseCases.MediaCases.ReelCase;
using Instagram.App.UseCases.MediaCases.StoryCase;
using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.MusicCase;
using Instagram.App.UseCases.SearchCase;
using Instagram.App.UseCases.Types.Search;
using Instagram.App.UseCases.UserCase.Followed;
using Instagram.App.UseCases.UserCase.Followers;
using Instagram.App.UseCases.UserCase.GetProfile;
using Instagram.App.UseCases.UserCase.SuggestFollowers;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Errors;

namespace Instagram.Presentation.GraphQL.Queries
{
    public class Query
    {
        [Authorize]
        [GraphQLDescription("Here you can get info about a user, but user needs to be authorized for using it")]
        public async Task<UserTypeOut> GetUserProfile([Service] IGetProfileCase getProfileCase, Guid userId)
        {
            var result = await getProfileCase.GetProfile(userId);

            return result is null ? throw new CustomGraphQlError("The user doesn't exist", System.Net.HttpStatusCode.NotFound) : result;
        }

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Here you can get followers about an user, but user needs to be authorized for using it")]
        public async Task<IEnumerable<FollowerType>?> GetFollowersFromUser([Service] IFollowerCase userCase, Guid userId)
        
           => await userCase.GetFollowersFromUser(userId);

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Here you can get followed about an user, but user needs to be authorized for using it")]
        public async Task<IEnumerable<FollowerType>?> GetUsersFollowedByOthers([Service] IFollowedCase userCase, Guid userId)
        
           =>  await userCase.GetUsersFollowedByOthers(userId);

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Here you can get suggestions about followers for an user, but user needs to be authorized for using it")]
        public async Task<IEnumerable<FollowerType>> GetSuggestionsFollowers([Service] ISuggestFollowersCase suggestFollowersCase, Guid userId) 
            => await suggestFollowersCase.GetSuggestionFollowers(userId);

        [Authorize]
        [GraphQLDescription("Search some song by its name, user needs to be authorized for using it")]
        public async Task<IEnumerable<SongSearchType>?> SearchSongByName([Service] IMusicCase musicCase, string trackName)
        {
            //Busca las canciones con el nombre de la cancion asociada
            return await musicCase.SearchSongByName(trackName);
        }

        [Authorize]
        [UsePaging]
        public async Task<IReadOnlyCollection<SearchType>?> SearchUsers([Service] ISeachCase searhCase,string input)
        {
            return await searhCase.SearchUsers(input);
        }

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Get feed post about users that one user followed it using user id, but user needs to be authorized")]
        public async Task<IReadOnlyList<PostTypeOut>> GetFeedPostByUserId([Service] IPostCase postCase, Guid userId)
        {
            return await postCase.GetFeedPostByUserId(userId);
        }

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Get feed reel about users that one user followed it using user id, but user needs to be authorized")]
        public async Task<IReadOnlyList<ReelTypeOut>> GetFeedReelByUserId([Service] IReelCase reelCase, Guid userId)
        {
            return await reelCase.GetFeedReelByUserId(userId);
        }

        [Authorize]
        [UsePaging]
        [GraphQLDescription("Get stories the useers that follows one user by userId, user needs to be authorized")]
        public async Task<IReadOnlyList<StoryTypeOut>> GetStoriesFromUsersByUserId([Service] IStoryCase storyCase, Guid userId)
        {
            return await storyCase.GetStoriesByUserId(userId);
        }

    }
}
