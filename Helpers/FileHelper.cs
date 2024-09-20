using HybridBookReader.Models;

namespace HybridBookReader.Helpers;

/// <summary>
/// Pomocná statická třída pro práci se soubory
/// </summary>
public static class FileHelper
{
    /// <summary>
    /// Doplňuje název souboru o cestu k internímu uložišti aplikace
    /// </summary>
    /// <param name="fileName">Název souboru</param>
    /// <returns>Cesta k souboru v interním uložišti</returns>
    public static string GetLocalFilePath(string fileName)
    {
        return Path.Combine(FileSystem.AppDataDirectory, fileName);
    }

    /// <summary>
    /// Doplňuje název souboru o cestu k uložišti dokumentů
    /// </summary>
    /// <param name="fileName">Název souboru</param>
    /// <returns>Cesta k souboru v uložišti dokumentů</returns>
    public static string GetDocumentsFilePath(string fileName)
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);
    }

    /// <summary>
    /// Přečte soubor a převede do pole bytů
    /// </summary>
    /// <param name="filePath">Cesta k souboru</param>
    /// <returns>Soubor jako pole bytů</returns>
    public static byte[] FileToByteArray(string filePath)
    {
        return File.ReadAllBytes(filePath);
    }

    /// <summary>
    /// Přečte soubor a převede do textového řetězce
    /// </summary>
    /// <param name="filePath">Cesta k souboru</param>
    /// <returns>Soubor jako textový řetězec</returns>
    public static string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }

    /// <summary>
    /// Ukládá textový řetězec jako soubor do uložiště dokumentů
    /// </summary>
    /// <param name="fileName">Název souboru</param>
    /// <param name="text">Text souboru</param>
    public static void SaveTextFileToDocumentsDirectory(string fileName, string text)
    {
        File.WriteAllText(GetDocumentsFilePath(fileName), text);
    }

    /// <summary>
    /// Vrací povolené koncovky souborů formátu EPUB pro různé platformy
    /// </summary>
    /// <returns>Povolené koncovky souborů</returns>
    public static FilePickerFileType GetEpubFileType()
    {
        return new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
{
   { DevicePlatform.iOS, new[] { "epub" } },
   { DevicePlatform.Android, new[] { "epub" } },
   { DevicePlatform.WinUI, new[] { "epub" } },
   { DevicePlatform.MacCatalyst, new[] { "epub" } },
});
    }
}
