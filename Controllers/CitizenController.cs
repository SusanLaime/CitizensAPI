using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private readonly List<Citizen> _citizensList;
    private readonly IConfiguration _configuration;
    private readonly CitizenBGService _citizenBGService;
    private readonly ILogger<CitizenController> _logger;

    public CitizenController(IConfiguration configuration, CitizenBGService citizenBGService, ILogger<CitizenController> logger)
    {
        _citizensList = new List<Citizen>();
        _configuration = configuration;
        _citizenBGService = citizenBGService;
        _logger = logger;

        try
        {
            List<string[]> data = CSVHelper.ReadCSV(_configuration["Data:Location"]);

            foreach (string[] row in data)
            {
                if (row.Length < 5)
                {
                    continue;
                }

                // Skip an optional header row if present, but keep real citizen data.
                if (!int.TryParse(row[2], out int ci))
                {
                    continue;
                }

                _citizensList.Add(new Citizen
                {
                    FirstName = row[0],
                    LastName = row[1],
                    CI = ci,
                    BloodGroup = row[3],
                    PersonalAsset = row[4]
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading citizens file.");
        }
    }

    //C: Create
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Citizen citizentoAdd)
    {
        try
        {
            List<CitizenBG> personalAssets = await _citizenBGService.GetCitizenBGs();
            if (personalAssets.Count == 0)
            {
                return StatusCode(503, "No personal assets are available from the external API.");
            }

            if (_citizensList.Any(c => c.CI == citizentoAdd.CI))
            {
                return Conflict("Citizen already exists with CI: " + citizentoAdd.CI);
            }

            CitizenBG selectedAsset = personalAssets[Random.Shared.Next(personalAssets.Count)];
            citizentoAdd.BloodGroup = BloodGroups.Allowed[Random.Shared.Next(BloodGroups.Allowed.Length)];
            citizentoAdd.PersonalAsset = string.IsNullOrWhiteSpace(selectedAsset.name) ? selectedAsset.id : selectedAsset.name;

            _citizensList.Add(citizentoAdd);
            List<string[]> data = new List<string[]>();

            for (int i = 0; i < _citizensList.Count; i++)
            { 
                string[] citizenData = new string[]
                {
                    _citizensList[i].FirstName,
                    _citizensList[i].LastName,
                    _citizensList[i].CI.ToString(),
                    _citizensList[i].BloodGroup,
                    _citizensList[i].PersonalAsset
                };
                data.Add(citizenData);
            }

            CSVHelper.WriteCSV(_configuration["Data:Location"], data);
            _logger.LogInformation("Citizen created with CI: {CitizenCI}", citizentoAdd.CI);
            return Ok(_citizensList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating citizen.");
            return StatusCode(500, "Internal server error.");
        }
    }

    //R: Read ALL / Retrieve ALL
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_citizensList);
    }

    //R: Read by ID / Retrieve by ID
    [HttpGet]
    [Route("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        Citizen foundCitizen = _citizensList.Find(c => c.CI == id);
        if (foundCitizen == null)
        {
            //Instead of Not FOund because it is too generic
            return Ok("Citizen not found with CI: " + id);
        }
        else
        {
            return Ok(foundCitizen);
        }

        
    }

    [HttpPut]
    [Route("{ci1}/{ci2}")]
    //U: Update
    public IActionResult Put([FromRoute] int ci1, [FromRoute] int ci2, [FromBody] Citizen citizenData)
    {
        try
        {
            Citizen citizenToUpdate = _citizensList.Find(c => c.CI == ci1);
            if (citizenToUpdate == null)
            {
                return Ok("Citizen not found with CI: " + ci1);
            }

            citizenToUpdate.FirstName = citizenData.FirstName;
            citizenToUpdate.LastName = citizenData.LastName;

            CSVHelper.WriteCSV(_configuration["Data:Location"], _citizensList.Select(c => new string[]
            {
                c.FirstName,
                c.LastName,
                c.CI.ToString(),
                c.BloodGroup,
                c.PersonalAsset
            }).ToList());

            _logger.LogInformation("Citizen updated with CI: {CitizenCI}", ci1);
            return Ok(_citizensList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating citizen with CI: {CitizenCI}", ci1);
            return StatusCode(500, "Internal server error.");
        }
    }

    [HttpDelete]
    [Route("{ci}")]
    //D: Delete
    public IActionResult Delete([FromRoute] int ci)
    {
        try
        {
            Citizen citizenToRemove = _citizensList.Find(c => c.CI == ci);

            if (citizenToRemove == null)
            {
                return Ok("Citizen not found with CI: " + ci);
            }

            _citizensList.Remove(citizenToRemove);
            CSVHelper.WriteCSV(_configuration["Data:Location"], _citizensList.Select(c => new string[]
            {
                c.FirstName,
                c.LastName,
                c.CI.ToString(),
                c.BloodGroup,
                c.PersonalAsset
            }).ToList());

            _logger.LogInformation("Citizen deleted with CI: {CitizenCI}", ci);
            return Ok(citizenToRemove);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting citizen with CI: {CitizenCI}", ci);
            return StatusCode(500, "Internal server error.");
        }
    }
}
