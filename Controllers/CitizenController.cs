using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class CitizenController : ControllerBase
{

    private List<Citizen> _citizensList;
    //Initialize the list
    public CitizenController()
    {
        _citizensList = new List<Citizen>();
        Citizen citizen1 = new Citizen();
        citizen1.FirstName = "Jonas";
        citizen1.LastName = "Brothers";
        citizen1.CI = 123456;
        citizen1.BloodGroup = "A+";
        citizen1.PersonalAsset = "House";
        _citizensList.Add(citizen1);

        _citizensList.Add(new Citizen
        {
            FirstName = "Julia",
            LastName = "Roberts",
            CI = 654321,
            BloodGroup = "A+",
            PersonalAsset = "House"
        });
    }

    //C: Create
    [HttpPost]
    public IActionResult Post([FromBody] Citizen citizentoAdd)
    {
        _citizensList.Add(citizentoAdd);
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
            return Ok(citizenToRemove);
        }
        
    }
}