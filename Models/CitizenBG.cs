public class CitizenBG
{
    public string id { get; set; }
    public string name { get; set; }

    public CitizenData data { get; set; }
}

public class CitizenData
{
    public string color { get; set; }
    public string capacity { get; set; }
}
