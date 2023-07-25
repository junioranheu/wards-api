using System.Text.Json.Serialization;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Application.UseCases.WardsHashtags.Shared.Input
{
    public sealed class WardHashtagInput
    {
        [JsonIgnore]
        public int WardHashtagId { get; set; }

        public int? WardId { get; set; }
 
        public int? HashtagId { get; set; }

        [JsonIgnore]
        public bool IsAtivo { get; set; } = true;

        [JsonIgnore]
        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}