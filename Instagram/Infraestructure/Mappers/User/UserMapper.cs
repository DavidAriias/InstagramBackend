using Instagram.App.UseCases.UserCase.Types;
using Instagram.config.helpers;
using Instagram.Domain.Entities.User;
using Instagram.Infraestructure.Data.Models.SQL;

namespace Instagram.Infraestructure.Mappers.User
{
    public static partial class UserMapper
    {
        public static UserEntity MapUserTypeInToUserEntity(UserTypeIn user) => new()
        {
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Username = user.Username,
            Password = EncryptHelper.GetPassEncrypt(user.Password),
            Name = user.Name,
            UserBirthday = user.Birthday,
            UserPronoun = user.Pronoun.ToString()
        };

        public static UserEntity MapUserDatumToUserEntity(UserDatum userDatum) => new()
        {
            Id = userDatum.Id,
            Username = userDatum.Username,
            Email = userDatum.Email,
            Password = userDatum.Password,
            PhoneNumber = userDatum.Phonenumber
        };

        public static UserTypeOut MapUserEntityToUserTypeOut(UserEntity user) => new()
        {
            Birthday = user.UserBirthday,
            Description = user.Description,
            Id = user.Id,
            ImageProfile = user.Imageprofile,
            Isverificated = user.Isverificated,
            Link = (user.Link is not null) ? MapLinkEntityToLinkType(user.Link) : null,
            Pronoun = user.UserPronoun,
            Username = user.Username,
            IsPrivate = user.IsPrivate

        };

        public static LinkType MapLinkEntityToLinkType(LinkEntity link) => new()
        {
            Link = link.Link,
            Title = link.Title
        };
        public static LinkEntity MapLinkTypeToLinkEntity(LinkType link) => new()
        {
            Title = link.Title,
            Link = link.Link
        };

        public static UserEntity MapUserStoredProcedureToUserEntity(UserStoredProcedure userStoredProcedure) => new()
        {
            Description = userStoredProcedure.Description,
            Id = userStoredProcedure.Id,
            Username = userStoredProcedure.Username,
            Imageprofile = userStoredProcedure.ImageProfile,
            IsPrivate = userStoredProcedure.IsPrivate,
            Isverificated = userStoredProcedure.IsVerificated,
            Name = userStoredProcedure.Name,
            UserPronoun = userStoredProcedure.UserPronoun,
            UserBirthday = userStoredProcedure.UserBirthday,
            Link = (userStoredProcedure.LinkLink is not null && userStoredProcedure.LinkTitle is not null) ? new LinkEntity
            {
                Link = userStoredProcedure.LinkLink,
                Title = userStoredProcedure.LinkTitle
            } : null
        };
    }
}
