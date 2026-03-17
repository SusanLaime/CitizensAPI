using System.Text.Json;

public class CitizenBGService
{
   private readonly HttpClient _httpClient;

   public CitizenBGService(IConfiguration configuration)
   {
       _httpClient = new HttpClient();
       _httpClient.BaseAddress = new Uri(configuration["External Services:ObjectsApi:BaseUrl"]);
   }

   public List<CitizenBG> GetCitizenBGs()
   {
       HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "objects");
       var response = _httpClient.Send(request);

       if (response.IsSuccessStatusCode)
       {
           string responseContent = response.Content.ReadAsStringAsync().Result;
           List<CitizenBG>? citizenBGs = JsonSerializer.Deserialize<List<CitizenBG>>(responseContent);
           return citizenBGs ?? new List<CitizenBG>();
       }
       else
       {
           throw new Exception($"Failed to retrieve data from the API. Status code: {response.StatusCode}");
       }
   }
}
