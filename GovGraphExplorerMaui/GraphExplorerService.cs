using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;

namespace GovGraphExplorerMaui;

public class GraphExplorerService {
    private HttpClient httpClient;
    private ILogger<AccessTokenService> logger;
    private readonly AccessTokenService accessTokenService;
    private string? accessToken = "";

    public GraphExplorerService(HttpClient _httpClient,
        ILoggerFactory loggerFactory,
        AccessTokenService accessTokenService) {
        httpClient = _httpClient;
        logger = loggerFactory.CreateLogger<AccessTokenService>();
        this.accessTokenService = accessTokenService;
    }

    private async Task GetAccessToken() {
        accessToken = await accessTokenService.GetAccessToken();
        logger.LogInformation("Access token: " + accessToken);
    }

    private HttpMethod GetHttpMethod(string method) {
        switch (method) {
            case "GET":
            return HttpMethod.Get;
            case "POST":
            return HttpMethod.Post;
            case "PATCH":
            return HttpMethod.Patch;
            case "PUT":
            return HttpMethod.Put;
            case "DELETE":
            return HttpMethod.Delete;
            default:
            return HttpMethod.Get;
        }
    }

    public async Task<(string, int, string)> GetGraphResponse(string endpoint, string httpVerb, Dictionary<string, string>? headerParameters, string? requestBody = null) {
        await GetAccessToken();

        // create request to url with optional body parameters
        var request = new HttpRequestMessage(GetHttpMethod(httpVerb), AppSettings.GraphApi + endpoint);
        if (!string.IsNullOrWhiteSpace(requestBody)) {
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Content = content;
            logger.LogInformation("Body: " + content);
            if (headerParameters is not null) {
                foreach (var headerParameter in headerParameters) {
                    try {
                        request.Content.Headers.Add(headerParameter.Key, headerParameter.Value);
                    } catch (Exception ex) {
                        logger.LogError(ex, "Error: " + ex.Message);
                    }
                }
            }
        }

        logger.LogInformation("Request: " + request.RequestUri);

        // add authorization header
        request.Headers.Add("Authorization", "Bearer " + accessToken);

        // add optional header parameters
        if (headerParameters is not null) {
            foreach (var headerParameter in headerParameters) {
                try {
                    request.Headers.Add(headerParameter.Key, headerParameter.Value);
                } catch (Exception ex) {
                    logger.LogError(ex, "Error: " + ex.Message);
                }
            }
        }

        // send request and return response
        HttpResponseMessage response = new();
        try {
            response = await httpClient.SendAsync(request);
        } catch (Exception ex) {
            logger.LogError(ex, "Error: " + ex.Message);
            return ($"Error: {ex.Message}", 0, accessToken!);
        }
        logger.LogInformation(response.StatusCode.ToString());
        var result = await response.Content.ReadAsStringAsync();
        logger.LogInformation(result);

        int count = 0;
        if (response.IsSuccessStatusCode) {

            if (string.IsNullOrWhiteSpace(result)) {
                return (response.ToString(), 0, accessToken!);
            }
            // get count of results if available

            // format json in pretty print
            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var formattedJson = result;
            JsonDocument? parsedResult = null;
            try {
                parsedResult = JsonDocument.Parse(result);
                formattedJson = JsonSerializer.Serialize(parsedResult, options);
                if (parsedResult.RootElement.TryGetProperty("value", out JsonElement value)) {
                    count = value.GetArrayLength();
                }
            } catch (Exception) {

            }

            return (formattedJson, count, accessToken!);
        } else {
            logger.LogWarning("Error: " + response.StatusCode);
            logger.LogWarning(result);

            var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            var formattedJson = result;
            try {
                formattedJson = JsonSerializer.Serialize(JsonDocument.Parse(result), options);
            } catch (Exception) {

            }

            return ($"Error: {formattedJson}", 0, accessToken!);
        }

    }
}
