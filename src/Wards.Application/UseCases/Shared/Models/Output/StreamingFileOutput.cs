namespace Wards.Application.UseCases.Shared.Models.Output
{
    public class StreamingFileOutput
    {
        public double PorcentagemCompleta { get; set; }

        public required byte[] Chunk { get; set; }
    }
}