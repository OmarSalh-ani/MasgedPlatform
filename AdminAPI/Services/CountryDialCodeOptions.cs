namespace AdminAPI.Services;

public class CountryDialCodeOptions
{
    public const string SectionName = "CountryDialCodes";
    public const string DefaultJsonFileName = "countires.json";

    public string JsonFileName { get; set; } = DefaultJsonFileName;
}
