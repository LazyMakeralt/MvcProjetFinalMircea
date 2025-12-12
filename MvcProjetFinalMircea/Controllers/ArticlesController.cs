using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcProjetFinalMircea.Data;
using MvcProjetFinalMircea.Models;

namespace MvcProjetFinalMircea.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ArticlesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // systeme de like
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleLike(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var like = await _context.ArticleLikes
                .FirstOrDefaultAsync(l => l.ArticleId == id && l.UserId == user.Id);

            if (like == null)
            {
                _context.ArticleLikes.Add(new ArticleLike
                {
                    ArticleId = id,
                    UserId = user.Id
                });
            }
            else
            {
                _context.ArticleLikes.Remove(like);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        // liste des articles
        public async Task<IActionResult> Index()
        {
            var articles = await _context.Articles
                .Include(a => a.Auteur)
                .Include(a => a.Likes)
                .OrderByDescending(a => a.DateCreation)
                .ToListAsync();

            return View(articles);
        }

         // details des articles
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles
                .Include(a => a.Auteur)
                .Include(a => a.Likes)
                .Include(a => a.Commentaires)
                    .ThenInclude(c => c.Auteur)
                .FirstOrDefaultAsync(a => a.Id == id);


            if (article == null) return NotFound();

            return View(article);
        }

        // METHODE create get
        [Authorize(Roles = "Admin,Auteur")]
        public IActionResult Create()
        {
            ViewData["AuteurId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }
        // METHODE CREATE POST
        [HttpPost]
        [Authorize(Roles = "Admin,Auteur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Titre,Contenu,AuteurId")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.DateCreation = DateTime.Now;

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuteurId"] = new SelectList(_context.Users, "Id", "UserName", article.AuteurId);
            return View(article);
        }

        // le edit pour modifier
        [Authorize(Roles = "Admin,Auteur")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles.FindAsync(id);
            if (article == null) return NotFound();

            ViewData["AuteurId"] = new SelectList(_context.Users, "Id", "UserName", article.AuteurId);
            return View(article);
        }

        // Edit post
        [HttpPost]
        [Authorize(Roles = "Admin,Auteur")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Contenu,AuteurId")] Article article)
        {
            if (id != article.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Articles.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Titre = article.Titre;
                existing.Contenu = article.Contenu;
                existing.AuteurId = article.AuteurId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuteurId"] = new SelectList(_context.Users, "Id", "UserName", article.AuteurId);
            return View(article);
        }

        // delete get
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Articles
                .Include(a => a.Auteur)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (article == null) return NotFound();

            return View(article);
        }

       // delete post
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article != null)
                _context.Articles.Remove(article);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterCommentaire(int articleId, string texte)
        {
            if (string.IsNullOrWhiteSpace(texte))
                return RedirectToAction("Details", new { id = articleId });

            var user = await _userManager.GetUserAsync(User);

            var commentaire = new Commentaire
            {
                ArticleId = articleId,
                Texte = texte,
                AuteurId = user.Id,
                DateCreation = DateTime.Now
            };

            _context.Commentaires.Add(commentaire);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = articleId });
        }
    }
}
