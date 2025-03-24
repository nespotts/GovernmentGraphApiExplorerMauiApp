
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GovGraphExplorerMaui;

public class AccessTokenService {
    private HttpClient httpClient;
    private ILogger<AccessTokenService> logger;
    private string tokenUrl = "";
    private string accessToken = "";

    public AccessTokenService(HttpClient _httpClient,
        ILoggerFactory loggerFactory) {
        httpClient = _httpClient;

        logger = loggerFactory.CreateLogger<AccessTokenService>();
    }

    public async Task<string?> GetAccessToken() {

        var parameters = new Dictionary<string, string>
            {
                { "client_id", AppSettings.ClientId },
                { "client_secret", AppSettings.ClientSecret },
                { "scope", AppSettings.Scope },
                { "grant_type", "client_credentials" }
            };

        var content = new FormUrlEncodedContent(parameters);
        tokenUrl = $"{AppSettings.TokenBaseUrl}/{AppSettings.TenantId}/oauth2/v2.0/token";
        HttpResponseMessage response = new();
        try {
            response = await httpClient.PostAsync(tokenUrl, content);
        } catch (Exception ex) {
            logger.LogError(ex, "Error: " + ex.Message);
            return $"Error: {ex.Message}";
        }

        if (response.IsSuccessStatusCode) {
            var result = await response.Content.ReadAsStringAsync();
            var tokenResponse = System.Text.Json.JsonDocument.Parse(result);
            accessToken = tokenResponse.RootElement.GetProperty("access_token").GetString()!;
            return tokenResponse.RootElement.GetProperty("access_token").GetString();
        } else {
            logger.LogWarning("Error: " + response.StatusCode);
            logger.LogWarning(await response.Content.ReadAsStringAsync());
            return null;
        }
    }
}
