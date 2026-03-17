using Newtonsoft.Json;

public class CitizenBGService
{
   private readonly HttpClient _httpClient;
   private readonly ILogger<CitizenBGService> _logger;

   public CitizenBGService(IConfiguration configuration, ILogger<CitizenBGService> logger)
   {
       _logger = logger;
       _httpClient = new HttpClient();
       _httpClient.BaseAddress = new Uri(configuration["External Services:ObjectsApi:BaseUrl"]);
   }

   public async Task<List<CitizenBG>> GetCitizenBGs()
   {
       try
       {
           HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "objects");
           _logger.LogInformation("External API request executed for citizen personal assets.");

           var response = await _httpClient.SendAsync(request);
           if (response.IsSuccessStatusCode)
           {
               string responseContent = await response.Content.ReadAsStringAsync();
               List<CitizenBG>? citizenBGs = JsonConvert.DeserializeObject<List<CitizenBG>>(responseContent);
               return citizenBGs ?? new List<CitizenBG>();
           }

           _logger.LogError("External API request failed with status code: {StatusCode}", response.StatusCode);
           throw new Exception($"Failed to retrieve data from the API. Status code: {response.StatusCode}");
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "External API request failed.");
           throw;
       }
   }
}
