namespace Wards.Application.UseCases.Shared.Models.Output
{
    public sealed class DropdownOptionOutput<T>
    {
        public required T Value { get; set; }
        public required string Label { get; set; }
    }
}