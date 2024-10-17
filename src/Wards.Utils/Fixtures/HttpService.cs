using System.Text;
using System.Text.Json;

namespace Wards.Utils.Fixtures;

public static class HttpService
{
    /// <summary>
    /// const string baseUrl = "https: //jsonplaceholder.typicode.com";
    /// 
    /// // Exemplo de uso do HttpService/Get;
    /// string getUrl = $"{baseUrl}/posts/1";
    /// var post = await SendRequestAsync<Teste>(getUrl, HttpMethod.Get);
    /// Console.WriteLine($"GET Request - Post: {post?.Title}");
    ///
    /// // Exemplo de uso do HttpService/Post
    /// string postUrl = $"{baseUrl}/posts";
    /// 
    /// var newPost = new Teste
    /// {
    ///     UserId = 1,
    ///     Title = "foo",
    ///     Body = "bar"
    /// };
    ///
    /// var createdPost = await SendRequestAsync<Teste>(postUrl, HttpMethod.Post, newPost);
    /// Console.WriteLine($"POST Request - Created Post: {createdPost?.Title}");
    ///
    /// // Classe de teste;
    /// public class Teste
    /// {
    ///     public int UserId { get; set; }
    ///     public int Id { get; set; }
    ///     public string Title { get; set; }
    ///     public string Body { get; set; }
    /// }
    /// </summary>
    public static async Task<T?> SendRequestAsync<T>(string url, HttpMethod method, object? content = null, int? timeOutInMinutes = 10) where T : class
    {
         using var _httpClient = new HttpClient
         {
             Timeout = TimeSpan.FromMinutes(timeOutInMinutes.GetValueOrDefault())
         };
        
        using var request = new HttpRequestMessage(method, url);

        if (content is not null)
        {
            var jsonContent = JsonSerializer.Serialize(content);
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(responseString, options: GetOptions());

        return result;
    }

    private static JsonSerializerOptions GetOptions() => new() { PropertyNameCaseInsensitive = true };
}
