using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace MvcProjetFinalMircea.Models
{
    public class Commentaire
    {
        public int Id { get; set; }

        [Required]
        public string Texte { get; set; } = default!;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public int ArticleId { get; set; }
        public Article Article { get; set; } = default!;

        public string AuteurId { get; set; } = default!;
        public IdentityUser Auteur { get; set; } = default!;
    }
}
