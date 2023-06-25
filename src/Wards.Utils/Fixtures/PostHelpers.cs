using System.Runtime.CompilerServices;
using static Wards.Utils.Fixtures.Convert;

internal static class PostHelpers
{

    /// <summary>
    /// Exemplo de streaming de um arquivo dividido em chunks;
    /// 
    /// Sites para testar/validar os chunks gerados:
    /// Base64 to .mp4: base64.guru/converter/decode/video;
    /// Base64 to .jpg: onlinejpgtools.com/convert-base64-to-jpg;
    /// </summary>
    public static async IAsyncEnumerable<byte[]> StreamFileEmChunks([EnumeratorCancellation] CancellationToken cancellationToken, string arquivo, int chunkSizeBytes)
    {
        if (arquivo is null || chunkSizeBytes < 1)
        {
            throw new Exception("Os parâmetros 'nomeArquivo' e 'chunkSizeBytes' não devem ser nulos");
        }

        Stream? stream = await ConverterPathParaStream(arquivo, chunkSizeBytes) ?? throw new Exception("Houve um erro interno ao buscar arquivo no servidor e convertê-lo em Stream");
        byte[]? buffer = new byte[chunkSizeBytes > stream.Length ? (int)stream.Length : (int)chunkSizeBytes];

        int bytesLidos;
        while (!cancellationToken.IsCancellationRequested && ((bytesLidos = await stream.ReadAsync(buffer)) > 0))
        {
            byte[]? chunk = new byte[bytesLidos];
            buffer.CopyTo(chunk, 0);

            yield return chunk;

            await Task.Delay(500, cancellationToken);
        }
    }
}