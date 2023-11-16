namespace Wards.Utils.Entities.Output
{
    public sealed class StreamingFileOutput
    {
        public double PorcentagemCompleta { get; set; }


        public required byte[] Chunk { get; set; }
    }
}