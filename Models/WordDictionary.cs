using System.Text.Json.Serialization;

namespace HybridBookReader.Models;

public partial class WordDictionary
{
    [JsonPropertyName("word")]
    public string Word { get; set; }
    [JsonPropertyName("meanings")]
    public List<Meaning> Meanings { get; set; }
}

public partial class Meaning
{
    [JsonPropertyName("partOfSpeech")]
    public string PartOfSpeech { get; set; }

    [JsonPropertyName("definitions")]
    public List<Definition> Definitions { get; set; }

    [JsonPropertyName("synonyms")]
    public List<string> SynonymsList { get; set; }

    [JsonPropertyName("antonyms")]
    public List<object> AntonymsList { get; set; }
    [JsonIgnore]
    public string Synonyms { get => string.Join(", ", SynonymsList); }

    [JsonIgnore]
    public string Antonyms { get => string.Join(", ", AntonymsList); }
}

public partial class Definition
{
    [JsonPropertyName("definition")]
    public string InnerDefinition { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("example")]
    public string Example { get; set; }
}