using System.Text.Json.Serialization;

namespace SteamAccountChecker.Models;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(FearApiResponse))]
[JsonSerializable(typeof(YoomaWsResponse))]
[JsonSerializable(typeof(YoomaApiResponse))]
public partial class JsonContext : JsonSerializerContext
{
}
