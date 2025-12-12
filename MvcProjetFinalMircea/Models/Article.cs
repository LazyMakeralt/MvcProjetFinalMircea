using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MvcProjetFinalMircea.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Titre { get; set; } = default!;

        [Required]
        public string Contenu { get; set; } = default!;

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public string AuteurId { get; set; } = default!;
        public IdentityUser? Auteur { get; set; }

        public ICollection<ArticleLike> Likes { get; set; } = new List<ArticleLike>();

        public ICollection<Commentaire> Commentaires { get; set; } = new List<Commentaire>();
    }
}
