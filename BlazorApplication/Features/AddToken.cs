using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace BlazorApplication.Features
{
    public class AddToken
    {
        public static async Task RequestAuthToken(IAccessTokenProvider _accessTokenProvider, HttpClient _client)
        {
            var requestToken = await _accessTokenProvider.RequestAccessToken();
            requestToken.TryGetToken(out var token);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Value);
            Console.WriteLine("Bearer " + token.Value);
            
        }
    }
}
