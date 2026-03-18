using Newtonsoft.Json;
using Serilog;

public class CitizenBGService
{
   private readonly HttpClient _httpClient;

   public CitizenBGService(IConfiguration configuration)
   {
       _httpClient = new HttpClient();
       string? baseUrl = configuration["External Services:ObjectsApi:BaseUrl"];
       Log.Debug("Configuring external citizen asset service with base URL {BaseUrl}.", baseUrl);
       _httpClient.BaseAddress = new Uri(baseUrl);
   }

   public async Task<List<CitizenBG>> GetCitizenBGs()
   {
       try
       {
           HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "objects");
           Log.Debug("Sending request to external API for citizen personal assets.");

           var response = await _httpClient.SendAsync(request);
           if (response.IsSuccessStatusCode)
           {
               string responseContent = await response.Content.ReadAsStringAsync();
               List<CitizenBG>? citizenBGs = JsonConvert.DeserializeObject<List<CitizenBG>>(responseContent);
               Log.Information("External API returned {AssetCount} personal assets.", citizenBGs?.Count ?? 0);
               return citizenBGs ?? new List<CitizenBG>();
           }

           Log.Warning("External API request failed with status code {StatusCode}.", response.StatusCode);
           throw new Exception($"Failed to retrieve data from the API. Status code: {response.StatusCode}");
       }
       catch (Exception ex)
       {
           Log.Error(ex, "External API request failed.");
           throw;
       }
   }
}
