using System.Text.Json.Serialization;
namespace HybridBookReader.Models;

public class Bookmark
{
    /// <summary>
    /// ID záložky
    /// </summary>
    public int BookmarkId { get; set; }
    /// <summary>
    /// ID knihy
    /// </summary>
    public int BookId { get; set; }
    /// <summary>
    /// Aktuální pozice čtení knihy
    /// </summary>
    public string CFIPosition { get; set; }
}
