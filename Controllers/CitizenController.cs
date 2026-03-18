using Microsoft.AspNetCore.Mvc;
using Serilog;
[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{
    private readonly List<Citizen> _citizensList;
    private readonly IConfiguration _configuration;
    private readonly CitizenBGService _citizenBGService;

    public CitizenController(IConfiguration configuration, CitizenBGService citizenBGService)
    {
        _citizensList = new List<Citizen>();
        _configuration = configuration;
        _citizenBGService = citizenBGService;

        try
        {
            string? dataLocation = _configuration["Data:Location"];
            Log.Debug("Loading citizens from CSV path {CsvPath}.", dataLocation);
            List<string[]> data = CSVHelper.ReadCSV(dataLocation);

            foreach (string[] row in data)
            {
                if (row.Length < 5)
                {
                    Log.Warning("Skipping malformed CSV row because it has {ColumnCount} columns.", row.Length);
                    continue;
                }

                // Skip an optional header row if present, but keep real citizen data.
                if (!int.TryParse(row[2], out int ci))
                {
                    Log.Debug("Skipping CSV row because CI value {CitizenCI} is not numeric.", row[2]);
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

            Log.Information("Loaded {CitizenCount} citizens from CSV.", _citizensList.Count);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error reading citizens file during controller initialization.");
        }
    }

    //C: Create
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CreateCitizenRequest citizenRequest)
    {
        try
        {
            Log.Debug("Creating citizen request received for CI {CitizenCI}.", citizenRequest.CI);
            List<CitizenBG> personalAssets = await _citizenBGService.GetCitizenBGs();
            if (personalAssets.Count == 0)
            {
                Log.Warning("Citizen creation stopped because the external API returned no personal assets.");
                return StatusCode(503, "No personal assets are available from the external API.");
            }

            if (_citizensList.Any(c => c.CI == citizenRequest.CI))
            {
                Log.Warning("Citizen creation rejected because CI {CitizenCI} already exists.", citizenRequest.CI);
                return Conflict("Citizen already exists with CI: " + citizenRequest.CI);
            }

            CitizenBG selectedAsset = personalAssets[Random.Shared.Next(personalAssets.Count)];
            Citizen citizentoAdd = new Citizen
            {
                FirstName = citizenRequest.FirstName,
                LastName = citizenRequest.LastName,
                CI = citizenRequest.CI,
                BloodGroup = BloodGroups.Allowed[Random.Shared.Next(BloodGroups.Allowed.Length)],
                PersonalAsset = string.IsNullOrWhiteSpace(selectedAsset.name) ? selectedAsset.id : selectedAsset.name
            };

            _citizensList.Add(citizentoAdd);
            List<string[]> data = new List<string[]>();

            Log.Debug("Preparing to write citizen data to CSV. Total citizens: {CitizenCount}.", _citizensList.Count);

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
            Log.Information("Citizen created with CI {CitizenCI} and personal asset {PersonalAsset}.", citizentoAdd.CI, citizentoAdd.PersonalAsset);
            return Ok(_citizensList);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error creating citizen with CI {CitizenCI}.", citizenRequest.CI);
            return StatusCode(500, "Internal server error.");
        }
    }

    //R: Read ALL / Retrieve ALL
    [HttpGet]
    public IActionResult Get()
    {
        Log.Information("Returning {CitizenCount} citizens.", _citizensList.Count);
        return Ok(_citizensList);
    }

    //R: Read by ID / Retrieve by ID
    [HttpGet]
    [Route("{id}")]
    public IActionResult Get([FromRoute] int id)
    {
        Log.Debug("Searching citizen with CI {CitizenCI}.", id);
        Citizen foundCitizen = _citizensList.Find(c => c.CI == id);
        if (foundCitizen == null)
        {
            Log.Warning("Citizen with CI {CitizenCI} was not found.", id);
            return Ok("Citizen not found with CI: " + id);
        }
        else
        {
            Log.Information("Citizen with CI {CitizenCI} found.", foundCitizen.CI);
            return Ok(foundCitizen);
        }
    }

    [HttpPut]
    [Route("{ci}")]
    //U: Update
    public IActionResult Put([FromRoute] int ci, [FromBody] UpdateCitizenRequest citizenData)
    {
        try
        {
            Log.Debug("Updating citizen with CI {CitizenCI}.", ci);
            Citizen citizenToUpdate = _citizensList.Find(c => c.CI == ci);
            if (citizenToUpdate == null)
            {
                Log.Warning("Citizen update skipped because CI {CitizenCI} was not found.", ci);
                return Ok("Citizen not found with CI: " + ci);
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

            Log.Information("Citizen updated with CI {CitizenCI}.", ci);
            return Ok(_citizensList);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error updating citizen with CI {CitizenCI}.", ci);
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
            Log.Debug("Deleting citizen with CI {CitizenCI}.", ci);
            Citizen citizenToRemove = _citizensList.Find(c => c.CI == ci);

            if (citizenToRemove == null)
            {
                Log.Warning("Citizen deletion skipped because CI {CitizenCI} was not found.", ci);
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

            Log.Information("Citizen deleted with CI {CitizenCI}.", ci);
            return Ok(citizenToRemove);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error deleting citizen with CI {CitizenCI}.", ci);
            return StatusCode(500, "Internal server error.");
        }
    }
}
