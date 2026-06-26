using System.Text.Json.Serialization;

namespace STS2ExclaimEverything.Utils;

public sealed class ExclaimSettings
{
    [JsonPropertyName("append_missing_terminal_exclamation")]
    public bool AppendMissingTerminalExclamation { get; set; } = true;
}