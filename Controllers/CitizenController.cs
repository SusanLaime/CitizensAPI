using Microsoft.AspNetCore.Mvc;
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

    //C: Create
    [HttpPost]
    public IActionResult Post([FromBody] Citizen citizentoAdd)
    {
        List<CitizenBG> personalAssets = _citizenBGService.GetCitizenBGs();
        if (personalAssets.Count == 0)
        {
            return StatusCode(503, "No personal assets are available from the external API.");
        }

        if (_citizensList.Any(c => c.CI == citizentoAdd.CI))
        {
            return Conflict("Citizen already exists with CI: " + citizentoAdd.CI);
        }

        CitizenBG selectedAsset = personalAssets[Random.Shared.Next(personalAssets.Count)];
        citizentoAdd.PersonalAsset = string.IsNullOrWhiteSpace(selectedAsset.name) ? selectedAsset.id : selectedAsset.name;

        _citizensList.Add(citizentoAdd);
        List<string[]> data = new List<string[]>();

        //Statefull to stateless app
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
        return Ok(_citizensList);
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
        Citizen citizenToUpdate = _citizensList.Find(c => c.CI == ci1);
        if (citizenToUpdate == null)
        {
            //Instead of Not FOund because it is too generic
            return Ok("Citizen not found with CI: " + ci1);
        }
        else
        {
            citizenToUpdate.CI = citizenData.CI;
            citizenToUpdate.FirstName = citizenData.FirstName;
            citizenToUpdate.LastName = citizenData.LastName;
            citizenToUpdate.BloodGroup = citizenData.BloodGroup;
            citizenToUpdate.PersonalAsset = citizenData.PersonalAsset;

            //UPDATE THE LIST
            CSVHelper.WriteCSV(_configuration["Data:Location"], _citizensList.Select(c => new string[]
            {
                c.FirstName,
                c.LastName,
                c.CI.ToString(),
                c.BloodGroup,
                c.PersonalAsset
            }).ToList());

            return Ok(_citizensList);
        }
    }

    [HttpDelete]
    [Route("{ci}")]
    //D: Delete
    public IActionResult Delete([FromRoute] int ci)
    {
        Citizen citizenToRemove = _citizensList.Find(c => c.CI == ci);
        
        if (citizenToRemove == null)
        {
            //Instead of Not FOund because it is too generic
            return Ok("Citizen not found with CI: " + ci);
        }
        else
        {
            _citizensList.Remove(citizenToRemove);
            //All the time, we overwrite the data of the file
            CSVHelper.WriteCSV(_configuration["Data:Location"], _citizensList.Select(c => new string[]
            {
                c.FirstName,
                c.LastName,
                c.CI.ToString(),
                c.BloodGroup,
                c.PersonalAsset
            }).ToList());
            return Ok(citizenToRemove);
        }
        
    }
}
