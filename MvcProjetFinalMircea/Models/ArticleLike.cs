using Microsoft.AspNetCore.Identity;

namespace MvcProjetFinalMircea.Models
{
    public class ArticleLike
    {
        public int Id { get; set; }

        public int ArticleId { get; set; }
        public Article Article { get; set; } = default!;

        public string UserId { get; set; } = default!;
        public IdentityUser User { get; set; } = default!;
    }
}
