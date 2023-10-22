using Instagram.Domain.Entities.User;
using Instagram.Infraestructure.Data.Models.SQL;

namespace Instagram.Infraestructure.Mappers.User
{
    public static partial class UserMapper
    {
        public static UserDescription MapUserEntityToUserDescription(UserEntity user) => new()
        {
            Description = user.Description,
            UserId = user.Id,
            Imageprofile = user.Imageprofile,
            Pronoun = user.UserPronoun
        };
        public static Birthday MapUserEntityToBirthday(UserEntity user) => new()
        {
            Birthdaydate = user.UserBirthday,
            UserId = user.Id
        };
        public static UserName MapUserEntityToUserName(UserEntity user) => new()
        {
            Name = user.Name!,
            UserId = user.Id
        };
        public static UserDatum MapUserEntityToUserDatum(UserEntity user) => new()
        {
            Email = user.Email,
            Password = user.Password,
            Username = user.Username
        };


    }
}
