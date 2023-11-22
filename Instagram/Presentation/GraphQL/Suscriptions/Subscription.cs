using HotChocolate.Authorization;
using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.SearchCase;
using Instagram.App.UseCases.Types.Search;
using Instagram.App.UseCases.UserCase.Followed;
using Instagram.App.UseCases.UserCase.Types.Events;

namespace Instagram.Presentation.GraphQL.Suscriptions
{
    public class Subscription
    {
        [Authorize]
        [Subscribe]
        public FollowedEventType? OnNotificationUserFollowed(
            [EventMessage] FollowedEventType eventFollow,
            Guid receiverId)
        {
            if (eventFollow.FollowedUserId == receiverId) return eventFollow;

            return null;
        }

        [Authorize]
        [Subscribe]
        public async Task<PostEventType?> OnNotificationCreatePost(
           [Service] IFollowedCase followedCase,
           [EventMessage] PostEventType eventFollow,
           Guid receiverId)
        {
            // Obtener la lista de usuarios seguidos por el usuario que creó la publicación.
            var followedUsers = await followedCase.GetUsersFollowedByOthers(eventFollow.UserId);

            // Comprobar si el receiverId coincide con alguno de los usuarios seguidos.
            foreach (var followedUser in followedUsers!)
            {
                if (followedUser.Id == receiverId)
                {
                    // Si el receptor está en la lista de usuarios seguidos, devuelve el evento.
                    return eventFollow;
                }
            }

            // Si el receptor no está en la lista de usuarios seguidos, devuelve null.
            return null;
        }

        [Authorize]
        [Subscribe]
        public async Task<ReelEventType?> OnNotificationCreateReel(
           [Service] IFollowedCase followedCase,
           [EventMessage] ReelEventType eventFollow,
           Guid receiverId
           )
        {
            // Obtener la lista de usuarios seguidos por el usuario que creó la publicación.
            var followedUsers = await followedCase.GetUsersFollowedByOthers(eventFollow.UserId);

            // Comprobar si el receiverId coincide con alguno de los usuarios seguidos.
            foreach (var followedUser in followedUsers!)
            {
                if (followedUser.Id == receiverId)
                {
                    // Si el receptor está en la lista de usuarios seguidos, devuelve el evento.
                    return eventFollow;
                }
            }

            // Si el receptor no está en la lista de usuarios seguidos, devuelve null.
            return null;
        }

        [Authorize]
        [Subscribe]
        public async Task<StoryEventType?> OnNotificationCreateStory(
           [Service] IFollowedCase followedCase,
           [EventMessage] StoryEventType eventFollow,
           Guid receiverId
            )
        {
            // Obtener la lista de usuarios seguidos por el usuario que creó la publicación.
            var followedUsers = await followedCase.GetUsersFollowedByOthers(eventFollow.UserId);

            // Comprobar si el receiverId coincide con alguno de los usuarios seguidos.
            foreach (var followedUser in followedUsers!)
            {
                if (followedUser.Id == receiverId)
                {
                    // Si el receptor está en la lista de usuarios seguidos, devuelve el evento.
                    return eventFollow;
                }
            }

            // Si el receptor no está en la lista de usuarios seguidos, devuelve null.
            return null;
        }

        [Authorize]
        [Subscribe]
        public LikeEventType? OnNotificationAddLike(
            [EventMessage] LikeEventType eventFollow,
            Guid receiverId
            )
        {
            if (receiverId == eventFollow.ReceiverId) return eventFollow;
            return null;
        }

        [Authorize]
        [Subscribe]
        public CommentEventType? OnNotificationAddComment(
            [EventMessage] CommentEventType eventFollow,
            Guid receiverId)
        {
            if (receiverId == eventFollow.ReceiverId) return eventFollow;

            return null;
        }

        [Authorize]
        [Subscribe]
        public ReplyEventType? OnNotificationAddReply(
            [EventMessage] ReplyEventType eventFollow,
            Guid receiverId)
        {
            if (receiverId == eventFollow.ReceiverId) return eventFollow;

            return null;
        }

    }
}
