using System.Text.Json.Serialization;

namespace Server;

internal class LoudnormStats
{
    [JsonPropertyName("input_i")]
    public string InputI { get; set; } = null!;

    [JsonPropertyName("input_lra")]
    public string InputLra { get; set; } = null!;

    [JsonPropertyName("input_tp")]
    public string InputTp { get; set; } = null!;

    [JsonPropertyName("input_thresh")]
    public string InputThresh { get; set; } = null!;

    [JsonPropertyName("target_offset")]
    public string TargetOffset { get; set; } = null!;
}