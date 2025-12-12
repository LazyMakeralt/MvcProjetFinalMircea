using System.ComponentModel.DataAnnotations;

namespace MvcProjetFinalMircea.Models
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        [Required]
        public string Titre { get; set; } = default!;

        [Required]
        public string Description { get; set; } = default!;

        public string? UrlImage { get; set; }
        public string? UrlProjet { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;
    }
}
