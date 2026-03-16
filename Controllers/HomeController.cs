using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    //C: Create
    [HttpPost]
    public void Post(){}

    //R: Read ALL / Retrieve ALL
    [HttpGet]
    public void Get(){}

    //R: Read by ID / Retrieve by ID
    [HttpGet]
    [Route("{id}")]
    public void Get(int id){}

    [HttpPut]
    //U: Update
    public void Put(){}

    [HttpDelete]
    //D: Delete
    public void Delete(){}
}