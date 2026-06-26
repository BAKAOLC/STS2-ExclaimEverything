using System.Text.Json.Serialization;

namespace STS2ExclaimEverything.Utils;

public sealed class ExclaimSettings
{
    [JsonPropertyName("append_missing_terminal_exclamation")]
    public bool AppendMissingTerminalExclamation { get; set; } = true;

    [JsonPropertyName("append_pure_numeric_terminal_exclamation")]
    public bool AppendPureNumericTerminalExclamation { get; set; }

    [JsonPropertyName("uppercase_convertible_characters")]
    public bool UppercaseConvertibleCharacters { get; set; }
}
