using System.ComponentModel.DataAnnotations;

namespace AspireLibrary.Models
{
    public class FineSettings
    {
        public int FineSettingsId { get; set; }

        [Range(0, 100)]
        public decimal FinePerDay { get; set; }

        [Range(0, 1000)]
        public decimal MaxFine { get; set; }
    }
}

