using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using Instagram.App.Auth;
using Instagram.App.UseCases.MediaCases.CaptionCase;
using Instagram.App.UseCases.MediaCases.CommentCase;
using Instagram.App.UseCases.MediaCases.LikeCase;
using Instagram.App.UseCases.MediaCases.LocationCase;
using Instagram.App.UseCases.MediaCases.PostCase;
using Instagram.App.UseCases.MediaCases.ReelCase;
using Instagram.App.UseCases.MediaCases.ReplyCase;
using Instagram.App.UseCases.MediaCases.StoryCase;
using Instagram.App.UseCases.MediaCases.TagsCase;
using Instagram.App.UseCases.MediaCases.Types.Posts;
using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media;
using Instagram.App.UseCases.MediaCases.Types.Shared.Media.Events;
using Instagram.App.UseCases.MediaCases.Types.Stories;
using Instagram.App.UseCases.NotificationCase;
using Instagram.App.UseCases.SMSCase;
using Instagram.App.UseCases.Types.Notification;
using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.EditProfile;
using Instagram.App.UseCases.UserCase.Followed;
using Instagram.App.UseCases.UserCase.Followers;
using Instagram.App.UseCases.UserCase.SignIn;
using Instagram.App.UseCases.UserCase.SignUp;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.App.UseCases.UserCase.Types.Events;
using Instagram.config.helpers;
using Instagram.Domain.Enums;
using Instagram.Domain.Errors;
using Instagram.Presentation.GraphQL.Suscriptions;
using System.Net;

namespace Instagram.Presentation.GraphQL.Mutations
{
    public class Mutation
    {
        [GraphQLDescription("Sign in session using username, email and password")]
        public async Task<AuthTypeOut> SignInUser([Service] ISignInCase signInCase, string stringForMethod, string password, AuthMethod authMethod)
        {
            var result = await signInCase.SignInUser(stringForMethod, password, authMethod) ??
                throw new CustomGraphQlError("User, phone number, email or password are wrong", HttpStatusCode.Unauthorized);
            return result;
        }

        [GraphQLDescription("Send a SMS to the user for confirm its phone number")]
        public async Task<SmsType> SendVerificationSMS([Service] ISmsCase smsCase, string phoneNumber)
        {
            if (!PhoneNumberHelper.IsValidPhoneNumber(phoneNumber)) throw new CustomGraphQlError("The phone number isn't valid", HttpStatusCode.BadRequest);

            await smsCase.SendVerifySMSAsync(phoneNumber, "Your verify code for your Instagram's account");
            return new SmsType(true, "We send a code a your phone number");
        }

        [GraphQLDescription("Confirm code send to the phone number")]
        public async Task<SmsType> ConfirmSMSCode([Service] ISmsCase smsCase, string phoneNumber, string code)
        {
            bool isVerificated = await smsCase.VerifySMSAsync(phoneNumber, code);

            if (isVerificated) return new SmsType(true, "Verification succesfully");
            else return new SmsType(false, "The code is wrong");
        }

        [Authorize]
        [GraphQLDescription("Sign up session with your email or phone number, if the chose is phone number you need first validate the phone number with the SMS")]
        public async Task<ResponseType<string>> SignUpUser([Service] ISignUpCase signUpCase, UserTypeIn user)
        {
            if (string.IsNullOrWhiteSpace(user.PhoneNumber) && string.IsNullOrWhiteSpace(user.Email))
            {
                throw new CustomGraphQlError("You need to send a phone number or email to begin your account", HttpStatusCode.BadRequest);
            }

            if (!string.IsNullOrWhiteSpace(user.PhoneNumber) && !PhoneNumberHelper.IsValidPhoneNumber(user.PhoneNumber))
            {
                throw new CustomGraphQlError("The phone number is not valid", HttpStatusCode.BadRequest);
            }

            if (!string.IsNullOrWhiteSpace(user.Email) && !EmailHelper.IsValidEmail(user.Email))
            {
                throw new CustomGraphQlError("The email input is not formatted as an email", HttpStatusCode.BadRequest);
            }

            if (!UsernameHelper.IsValidUsername(user.Username))
            {
                throw new CustomGraphQlError("The username must be between 1 and 30 characters and it doesn't have blank spaces", HttpStatusCode.Conflict);
            }

            if (!BirthdayHelper.IsValidAge(user.Birthday))
            {
                throw new CustomGraphQlError("You can't make an Instagram account", HttpStatusCode.Conflict);
            }

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                throw new CustomGraphQlError("The email is empty", HttpStatusCode.BadRequest);
            }

            return await signUpCase.CreateUser(user);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the birthday of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>?> UpdateBirthday([Service] IEditProfileCase editProfileCase, DateOnly birthday, Guid userId)
        {
            //Valida que sea mayor a 13 años
            if (!BirthdayHelper.IsValidAge(birthday)) throw new CustomGraphQlError("You can't change your birthday to less 13 years", HttpStatusCode.Conflict);

            var result = await editProfileCase.UpdateBirthday(birthday, userId);

            if (result.StatusCode == HttpStatusCode.OK) return result;

            else throw new CustomGraphQlError(result.Message, result.StatusCode);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the pass of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdatePassword([Service] IEditProfileCase editProfileCase, string password, Guid userId)
        {
            /*
             * Valida que la contraseña sea de longitud de min 8 caracteres y de max 16,
             * que contenga mayusculas y minusculas
             */
            if (!PassHelper.IsValidPass(password)) throw new CustomGraphQlError("The password must be at least 8 characters, use mayus, minus and symbols", HttpStatusCode.BadRequest);

            return await editProfileCase.UpdatePassword(password, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the username of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateUsername([Service] IEditProfileCase editProfileCase, string username, Guid userId)
        {

            /*Valida que el username sea de longitud maxima de 30 caracteres y al menos sea de 1 caracter
             * y que no contenga espacios en blanco
             */
            if (!UsernameHelper.IsValidUsername(username)) throw new CustomGraphQlError("The username must be between 1 and 30 characters and it doesn't have blank spaces", HttpStatusCode.Conflict);

            return await editProfileCase.UpdateUsername(username, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the link of an user, but the user needs to be authorized")]
        public async Task<ResponseType<LinkType?>> UpdateLink([Service] IEditProfileCase editProfileCase,LinkType link, Guid userId)
        {
            if (!LinkHelper.IsLink(link.Link)) throw new CustomGraphQlError("The link doesn't have the format for being an URL", HttpStatusCode.BadRequest);

            if (link.Link.Length > 200) throw new CustomGraphQlError("The max lenght must be 200 characters for link", HttpStatusCode.BadRequest);

            if (link.Title.Length > 150) throw new CustomGraphQlError("The max lenght must be 150 characters for title", HttpStatusCode.BadRequest);

            return await editProfileCase.UpdateLink(link, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the biography of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateBiography([Service] IEditProfileCase editProfileCase, string bio, Guid userId)
        {
            if (bio.Length > 150) throw new CustomGraphQlError("The max lenght must be 150 characters for bio", HttpStatusCode.BadRequest);

            return await editProfileCase.UpdateBiography(bio, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the name of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateName([Service] IEditProfileCase editProfileCase, string name, Guid userId)
        {
            if (name.Length > 80) throw new CustomGraphQlError("The max lenght must be 80 characteres for name", HttpStatusCode.BadRequest);

            return await editProfileCase.UpdateName(name, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the pronoun of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdatePronoun([Service] IEditProfileCase editProfileCase, PronounEnum pronoun, Guid userId)

            => await editProfileCase.UpdatePronoun(pronoun, userId);

        [Authorize]
        [GraphQLDescription("Here you can update the image profile of an user, but the user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateImageProfile([Service] IEditProfileCase editProfileCase, IFile image, Guid userId)
        {
            return await editProfileCase.UpdateImageProfile(image, userId);
        }


        [GraphQLDescription("Here you can follow to an user, but you need to send the id's from users, and the follow user needs to be authorized")]
        public async Task<FollowedEventType> FollowToUser(
            [Service] ITopicEventSender eventSender,
            [Service] IFollowerCase followerCase,
            [Service] IFollowedCase followedCase,
            [Service] INotificationCase notificationCase,
            Guid followerId,
            Guid followedUserId)
        {
            //Validacion para que el usuario no pueda seguirse a si mismo
            if (followerId == followedUserId) throw new CustomGraphQlError("The user can't follow to itself", HttpStatusCode.BadRequest);

            //Validacion para que no pueda seguir mas de una vez al mismo usuario
            bool isFollowed = await followerCase.IsUserFollowToOther(followerId, followedUserId);

            if (isFollowed) throw new CustomGraphQlError("You already follow it", HttpStatusCode.Conflict);

            var result = await followedCase.FollowToUser(followerId, followedUserId);

            await notificationCase.SendPushNotificationToUser($"{result.FollowerName} starts follow you", followedUserId);

            //Mandando la suscripcion del usuario destino para que sea notificado de que alguien lo sigue
            await eventSender.SendAsync(nameof(Subscription.OnNotificationUserFollowed), result);

            return result;
        }

        [Authorize]
        [GraphQLDescription("Here you can unfollow an user, but you need to send the id's from users, and the follow user needs to be authorized")]
        public async Task<ResponseType<Guid>> UnfollowUser([Service] IFollowerCase followerCase, Guid userId, Guid followerId)
        {
            if (userId == followerId) throw new CustomGraphQlError("The user id can't be same follower id", HttpStatusCode.BadRequest);

            bool isFollowed = await followerCase.IsUserFollowToOther(followerId, userId);

            if (!isFollowed) throw new CustomGraphQlError("You can't do to unfollow to an user that you don't follow", HttpStatusCode.Conflict);

            return await followerCase.UnfollowUser(followerId, userId);
        }

        [Authorize]
        [GraphQLDescription("Her you can change user's privacity account, user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateIsPrivateProfile([Service] IEditProfileCase editProfileCase, Guid userId)

            => await editProfileCase.UpdateIsPrivateProfile(userId);

        [Authorize]
        [GraphQLDescription("Here you can change user's verificatation account, user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateIsVerificateProfile([Service] IEditProfileCase editProfileCase, Guid userId)

            => await editProfileCase.UpdateIsVerificateProfile(userId);


        [Authorize]
        [GraphQLDescription("Here you can create a post, but the user needs to be authorized for using it.")]
        public async Task<PostEventType> CreatePost(
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            [Service] IPostCase postCase,
            PostTypeIn post)
        {
            // Comprueba la longitud de la leyenda.
            if (post.Caption?.Length > 2200)
            {
                throw new CustomGraphQlError("The caption must be a maximum of 2200 characters", HttpStatusCode.Conflict);
            }

            // Comprueba la cantidad de etiquetas.
            if (post.Tags?.Count > 30)
            {
                throw new CustomGraphQlError("The maximum number of tags is 30", HttpStatusCode.Conflict);
            }

            // Comprueba la cantidad de medios.
            if (post.Media?.Count > 10 || post.Media?.Count < 1)
            {
                throw new CustomGraphQlError("The maximum number of media is 10 and the minimum is 1", HttpStatusCode.Conflict);
            }

            // Verifica la validez de etiquetas y menciones.
            bool areTags = post.Tags is null || TagHelper.AreAllHashtagsValid(post.Tags);
            bool areMentions = post.Mentions is null || !post.Mentions
                .Any(m => !MentionHelper.IsMentionValid(m.UserId));

            if (!areTags || !areMentions)
            {
                throw new CustomGraphQlError("Tags or mentions are not valid", HttpStatusCode.BadRequest);
            }

            // Crea la publicación.
            var result = await postCase.CreatePostAsync(post);

            /// Envía notificaciones push a los seguidores sobre la nueva publicación.
            await notificationCase.SendPushNotificationToFollowers($"{post.Username} has shared a post", post.UserId);

            // Envía un evento de suscripción para la nueva publicación.
            await eventSender.SendAsync(nameof(Subscription.OnNotificationCreatePost), result);

            return result;
        }


        [Authorize]
        [GraphQLDescription("Here you can delete a post type, but the user needs to be authorized to do it.")]
        public async Task<ResponseType<string>> DeletePost([Service] IPostCase postCase, Guid userId, string mediaId)
        {
            // Validar si mediaId es nulo o vacío
            if (string.IsNullOrEmpty(mediaId))
            {
                throw new CustomGraphQlError("The media id must be provided", HttpStatusCode.BadRequest);
            }

            // Llamar a la función que realiza la eliminación del post
            return await postCase.DeletePostAsync(mediaId, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can update tags for a media, but the user needs to be authorized to do it.")]
        public async Task<ResponseType<IReadOnlyList<string>>> UpdateTagsToMedia([Service] ITagsCase tagsCase, TagsTypeIn tagsType)
        {
            // Validar si postId es nulo o vacío
            if (string.IsNullOrEmpty(tagsType.MediaId))
            {
                throw new CustomGraphQlError("The media id must be provided", HttpStatusCode.BadRequest);
            }

            // Validar si no se proporcionan etiquetas (tags)
            if (tagsType.Tags == null || tagsType.Tags.Count < 1)
            {
                throw new CustomGraphQlError("To update tags, at least one tag must be provided", HttpStatusCode.BadRequest);
            }

            // Comprueba si todas las etiquetas son válidas utilizando el helper TagHelper.
            bool areTagsValid = TagHelper.AreAllHashtagsValid(tagsType.Tags);

            // Si las etiquetas no son válidas, devuelve una respuesta de conflicto.
            if (!areTagsValid) throw new CustomGraphQlError("Tags are not valid, the format is wrong", HttpStatusCode.BadRequest);

            // Llamar a la función que realiza la actualización de etiquetas (tags)
            return await tagsCase.UpdateTagsAsync(tagsType);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the caption about some media like post or reel (story not supported), user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateCaptionToMedia([Service] ICaptionCase captionCase, CaptionTypeIn captionType)
        {
            if (captionType.ContentType == ContentEnum.Story) throw new CustomGraphQlError("Story not suported", HttpStatusCode.BadRequest);

            return await captionCase.UpdateCaption(captionType);
        }

        [Authorize]
        [GraphQLDescription("Here you can update the location about some media like post or reel (story not supported), user needs to be authorized")]
        public async Task<ResponseType<string>> UpdateLocationToMedia([Service] ILocationCase locationCase, LocationType locationType, string mediaId, ContentEnum contentEnum)
        {
            if (string.IsNullOrEmpty(mediaId)) throw new CustomGraphQlError("The media id must be provided", HttpStatusCode.BadRequest);
            if (contentEnum == ContentEnum.Story) throw new CustomGraphQlError("Story not suported", HttpStatusCode.BadRequest);

            return await locationCase.UpdateLocation(locationType, mediaId, contentEnum);
        }

        [Authorize]
        [GraphQLDescription("Here you can add like to post, reel or story, but the user needs to be authorized")]
        public async Task<LikeEventType> AddLikeToMedia(
            [Service] ILikeCase likeCase,
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            LikeTypeIn like)
        {
            var result = await likeCase.AddLikeToMedia(like);

            await notificationCase.SendPushNotificationToUser($"{result.SenderUsername} has given to like to your post", result.ReceiverId);

            await eventSender.SendAsync(nameof(Subscription.OnNotificationAddLike), result);

            return result;
        }

        [Authorize]
        [GraphQLDescription("Here you can remove a like to post, reel or story, but the user needs to be authorized")]
        public async Task<ResponseType<string>> RemoveLikeFromMedia([Service] ILikeCase likeCase, LikeTypeIn like)
        {
            return await likeCase.RemoveLikeToMedia(like);
        }

        [Authorize]
        [GraphQLDescription("Add a comment to post, reel or story, " +
            "The 'commentId' field is not required.")]
        public async Task<CommentEventType> AddCommentToMedia(
            [Service] ICommentCase commentCase,
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            CommentTypeIn comment)
        {
            if (string.IsNullOrEmpty(comment.Comment)) throw new CustomGraphQlError("Comment field is missing", HttpStatusCode.BadRequest);

            var result = await commentCase.CreateComment(comment);

            await notificationCase.SendPushNotificationToUser($"{result.SenderUsername} comments your post", result.ReceiverId);

            await eventSender.SendAsync(nameof(Subscription.OnNotificationAddComment), result);

            return result;
        }

        [Authorize]
        [GraphQLDescription("Remove a comment from post, reel, or story. The 'commentId' field is required.")]
        public async Task<ResponseType<string>> RemoveCommentFromMedia([Service] ICommentCase commentCase, CommentTypeIn comment)
        {
            if (string.IsNullOrEmpty(comment.CommentId)) throw new CustomGraphQlError("Comment ID field is missing", HttpStatusCode.BadRequest);
            return await commentCase.DeleteComment(comment);
        }

        [Authorize]
        [GraphQLDescription("Add a reply to media content such as post or reel (story not supported). " +
            "Reply ID is not required, but user authorization is.")]
        public async Task<ReplyEventType> AddReplyToMedia(
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            [Service] IReplyCase replyCase,
            ReplyType replyType)
        {
            var result = await replyCase.CreateReply(replyType);

            await notificationCase.SendPushNotificationToUser($"{result.SenderUsername} has replied to comment", result.ReceiverId);

            await eventSender.SendAsync(nameof(Subscription.OnNotificationAddReply), result);

            return result;
        }

        [Authorize]
        [GraphQLDescription("Remove a reply to media content such as post or reel (story not supported)." +
            " Reply ID is required and text field is not required,but user authorization is.")]
        public async Task<ResponseType<string>> RemoveReplyFromMedia([Service] IReplyCase replyCase, ReplyType replyType)
        {
            if (replyType.ReplyId is null) throw new CustomGraphQlError("Reply ID field is missing", HttpStatusCode.BadRequest);
            return await replyCase.DeleteReply(replyType);
        }

        [Authorize]
        [GraphQLDescription("Here you can create a reel type, but the user needs to be authorized to do it.")]
        public async Task<ReelEventType> CreateReel(
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            [Service] IReelCase reelCase,
            ReelTypeIn reelType)
        {
            // Verifica si se proporciona el archivo 'File' para el reel, si no, devuelve un error de solicitud incorrecta.
            if (reelType.File is null)
            {
                throw new CustomGraphQlError("Missing file for reel", HttpStatusCode.BadRequest);
            }

            // Verifica si la leyenda excede la longitud máxima permitida.
            if (reelType.Caption?.Length > 2200)
            {
                throw new CustomGraphQlError("The caption must be a maximum of 2200 characters", HttpStatusCode.Conflict);
            }

            // Verifica el número de etiquetas.
            if (reelType.Tags?.Count > 30)
            {
                throw new CustomGraphQlError("The maximum number of tags is 30", HttpStatusCode.Conflict);
            }

            // Verifica la validez de etiquetas y menciones.
            bool areTags = reelType.Tags is null || TagHelper.AreAllHashtagsValid(reelType.Tags);
            bool areMentions = reelType.Mentions is null || !reelType.Mentions
                .Any(m => !MentionHelper.IsMentionValid(m.UserId));

            if (!areTags || !areMentions)
            {
                throw new CustomGraphQlError("Tags or mentions are not valid", HttpStatusCode.BadRequest);
            }

            // Crea el reel.
            var result = await reelCase.CreateReelAsync(reelType);

            // Envía notificaciones push a los seguidores sobre el nuevo reel.
            await notificationCase.SendPushNotificationToFollowers($"{reelType.Username} has shared a reel", reelType.UserId);

            // Envía un evento de suscripción para el nuevo reel.
            await eventSender.SendAsync(nameof(Subscription.OnNotificationCreateReel), result);

            return result;
        }


        [Authorize]
        [GraphQLDescription("Here you can delete a reel type, but the user needs to be authorized to do it.")]
        public async Task<ResponseType<string>> DeleteReel([Service] IReelCase postCase, Guid userId, string mediaId)
        {
            // Validar si mediaId es nulo o vacío
            if (string.IsNullOrEmpty(mediaId))
            {
                throw new CustomGraphQlError("The media id must be provided", HttpStatusCode.BadRequest);
            }

            return await postCase.DeleteReelAsync(mediaId, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you can create a story type, but the user needs to be authorized to do it.")]
        public async Task<StoryEventType> CreateStory(
            [Service] ITopicEventSender eventSender,
            [Service] INotificationCase notificationCase,
            [Service] IStoryCase storyCase,
            StoryTypeIn storyType)
        {
            var result = await storyCase.CreateStory(storyType);

            await notificationCase.SendPushNotificationToFollowers($"{storyType.Username} has shared a story", storyType.UserId);

            await eventSender.SendAsync(nameof(Subscription.OnNotificationCreateStory), result);

            return result;
        }

        [Authorize]
        [GraphQLDescription("Here you can delete a story type, but the user needs to be authorized to do it.")]
        public async Task<ResponseType<string>> DeleteStory(
            [Service] IStoryCase storyCase,
            Guid userId,
            string mediaId)
        {
            // Validar si mediaId es nulo o vacío
            if (string.IsNullOrEmpty(mediaId))
            {
                throw new CustomGraphQlError("The media id must be provided", HttpStatusCode.BadRequest);
            }

            return await storyCase.DeleteStory(mediaId, userId);
        }

        [Authorize]
        [GraphQLDescription("Here you add a device token, but the user needs to be authorized to do it.")]
        public async Task<ResponseType<string>> RegisterDeviceToken(
            [Service] INotificationCase notificationCase,
            DeviceTokenType deviceToken
            )
        {
            return await notificationCase.RegisterDeviceToken(deviceToken);
        }

        [Authorize]
        [GraphQLDescription("Here you must send refresh token when user closes session in the app, the access token is optional if you send it")]
        public async Task<ResponseType<string>> CloseSession([Service] IAuthService authService, AuthTypeIn auth)
        {
            return await authService.CloseSession(auth);
        }
        
    }
}
