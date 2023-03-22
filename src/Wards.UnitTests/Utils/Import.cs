using Microsoft.AspNetCore.Http;

namespace Wards.UnitTests.Utils
{
    public class ImportTestsBase
    {
        public static void CriarFormFile(string filename, out MemoryStream stream, out FormFile formFile)
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory!, $"Assets\\{filename}");
            stream = new MemoryStream(File.ReadAllBytes(path).ToArray());
            formFile = new FormFile(stream, 0, stream.Length, "streamFile", path.Split(@"\").Last());
        }
    }
}