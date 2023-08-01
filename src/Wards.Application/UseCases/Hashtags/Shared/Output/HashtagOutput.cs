namespace Wards.Application.UseCases.Hashtags.Shared.Output
{
    public sealed class HashtagOutput
    {
        public int HashtagId { get; set; }

        public string Tag { get; set; } = string.Empty;

        public bool IsAtivo { get; set; } = true;

        public DateTime Data { get; set; }
    }
}