using Instagram.App.UseCases.Types.Shared;
using Instagram.App.UseCases.UserCase.Types;
using Instagram.Domain.Enums;

namespace Instagram.App.UseCases.UserCase.EditProfile
{
    public interface IEditProfileCase
    {
        public Task<ResponseType<string>> UpdateBirthday(DateOnly birthday, Guid userId);
        public Task<ResponseType<string>> UpdatePassword(string pass, Guid userId);
        public Task<ResponseType<string>> UpdateUsername(string username, Guid userId);
        public Task<ResponseType<LinkType?>> UpdateLink(LinkType link, Guid userId);
        public Task<ResponseType<string>> UpdateBiography(string bio, Guid userId);
        public Task<ResponseType<string>> UpdateName(string name, Guid userId);
        public Task<ResponseType<string>> UpdatePronoun(PronounEnum pronoun, Guid userId);
        public Task<ResponseType<string>> UpdateImageProfile(IFile imageProfile, Guid userId);
        public Task<ResponseType<string>> UpdateIsPrivateProfile(Guid userId);
        public Task<ResponseType<string>> UpdateIsVerificateProfile(Guid userId);

    }
}
