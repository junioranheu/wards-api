using Wards.Domain.Entities;

namespace Wards.Application.UseCases.WardsHashtags.Shared.Output
{
    public sealed class WardHashtagOutput
    {
        public int? WardId { get; set; }

        public int? HashtagId { get; set; }
        public Hashtag? Hashtags { get; init; }
    }
}