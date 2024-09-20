using System.Text.Json.Serialization;

namespace HybridBookReader.Models
{
    /// <summary>
    /// Třída knihy
    /// </summary>
    public class Book
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonIgnore]
        public int BookId { get; set; }
        /// <summary>
        /// Název
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }
        /// <summary>
        /// Jméno autora
        /// </summary>
        [JsonPropertyName("author")]
        public string Author { get; set; }
        /// <summary>
        /// Popisek
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }
        /// <summary>
        /// Souborová cesta ke knize
        /// </summary>
        [JsonPropertyName("bookPath")]
        public string BookPath { get; set; }
        /// <summary>
        /// Souborová cesta k obrázku obálky knihy
        /// </summary>
        [JsonPropertyName("coverImagePath")]
        public string CoverImagePath { get; set; }
        /// <summary>
        /// Aktuální pozice čtení knihy
        /// </summary>
        [JsonPropertyName("cfiPosition")]
        public string CFIPosition { get; set; }
        /// <summary>
        /// Počet stran knihy
        /// </summary>
        [JsonPropertyName("pagesCount")]
        public int PagesCount { get; set; }
        /// <summary>
        /// Právě čtená stránka
        /// </summary>
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }
        /// <summary>
        /// Datum posledního čtení jako textový řetězec
        /// </summary>
        [JsonPropertyName("lastOpened")]
        public string LastOpened { get; set; }
        /// <summary>
        /// Počet přečtených stran v procentech
        /// </summary>
        [JsonIgnore]
        public double PercentageProgress
        {
            get
            {
                if (CurrentPage > 0) 
                    return Convert.ToDouble(CurrentPage) / Convert.ToDouble(PagesCount);
                else 
                    return 0;
            }
        }
        /// <summary>
        /// Počet přečtených stran v procentech jako textový řetězec
        /// </summary>
        [JsonIgnore]
        public string PercentageProgressAsText { get => $"{Convert.ToInt32(PercentageProgress * 100)}%"; }

        public Book()
        {
        }

        /// <summary>
        /// Datum posledního čtení
        /// </summary>
        public DateTime LastOpenedAsDate()
        {
            DateTime date;
            if (!string.IsNullOrEmpty(LastOpened) && DateTime.TryParse(LastOpened, out date))
                return date;
            else
                return DateTime.MinValue;
        }
    }
}
