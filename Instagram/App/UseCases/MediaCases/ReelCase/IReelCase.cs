﻿using Instagram.App.UseCases.MediaCases.Types.Reels;
using Instagram.App.UseCases.Types.Shared;

namespace Instagram.App.UseCases.MediaCases.ReelCase
{
    public interface IReelCase
    {
        public Task<ReelEventType> CreateReelAsync(ReelTypeIn reelIn);
        public Task<ResponseType<string>> DeleteReelAsync(string reelId, Guid userId);
        public Task<IReadOnlyList<ReelTypeOut>> GetFeedReelByUserId(Guid userId);
    }
}
