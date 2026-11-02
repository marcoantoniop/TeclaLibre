using System.Text.Json.Serialization;

namespace TeclaLibre.Models;

public sealed class AppConfiguration
{
    [JsonRequired]
    public int SchemaVersion { get; set; } = 1;

    [JsonRequired]
    public AppSettings Settings { get; set; } = new();

    [JsonRequired]
    public List<MappingRule> Rules { get; set; } = new();
}