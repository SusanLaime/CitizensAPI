using Newtonsoft.Json;

public class CitizenBGService
{
   private readonly HttpClient _httpClient;

   public CitizenBGService(IConfiguration configuration)
   {
       _httpClient = new HttpClient();
       _httpClient.BaseAddress = new Uri(configuration["External Services:ObjectsApi:BaseUrl"]);
   }

   public async Task<List<CitizenBG>> GetCitizenBGs()
   {
       HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "objects");
       //await will automatically force to respond the server: ASYNC await
       var response = await _httpClient.SendAsync(request);

       if (response.IsSuccessStatusCode)
       {
           string responseContent = response.Content.ReadAsStringAsync().Result;
           //Change library to Newtonsoft.Json for deserialization
           List<CitizenBG>? citizenBGs = JsonConvert.DeserializeObject<List<CitizenBG>>(responseContent);
           return citizenBGs ?? new List<CitizenBG>();
       }
       else
       {
           throw new Exception($"Failed to retrieve data from the API. Status code: {response.StatusCode}");
       }
   }
}
