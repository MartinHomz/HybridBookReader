using System.Text.Json.Serialization;

namespace HybridBookReader.Models;

/// <summary>
/// Přenosová třída dat aplikace (pro přenos mezi DB a JSON)
/// </summary>
public class TransferModel
{
    /// <summary>
    /// Knihovna
    /// </summary>
    [JsonPropertyName("library")]
    public List<Book> Books { get; set; } = new List<Book>();
    /// <summary>
    /// Záložky
    /// </summary>
    [JsonPropertyName("bookmarks")]
    public List<Bookmark> Bookmarks { get; set; } = new List<Bookmark>();
}
