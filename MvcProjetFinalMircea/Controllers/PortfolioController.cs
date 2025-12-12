using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcProjetFinalMircea.Data;
using MvcProjetFinalMircea.Models;

namespace MvcProjetFinalMircea.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public PortfolioController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var projets = await _context.PortfolioItems
                .OrderByDescending(p => p.DateCreation)
                .ToListAsync();

            return View(projets);
        }

        //  create pour les admins seulement
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // create projet dans le portfolio
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PortfolioItem item, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    var uploadsPath = Path.Combine(_env.WebRootPath, "uploads/portfolio");
                    Directory.CreateDirectory(uploadsPath);

                    var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    item.UrlImage = "/uploads/portfolio/" + fileName;
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
    }
}
