using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;

        public GenreController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Genres.Where(g=>g.IsDeleted == false).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null)
                return View("Error404");

            Genre genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == Id && g.IsDeleted == false);

            if (genre == null)
                return View("Error404");

            return View(genre);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Genre genre)
        {
            if (!ModelState.IsValid)
                return View(genre);

            if(await _context.Genres.AnyAsync(g=>g.Name.ToLower() == genre.Name.ToLower() && g.IsDeleted == false))
            {
                ModelState.AddModelError("Name", $"Bu {genre.Name} Adda Artiq Genre Var");
                return View(genre);
            }

            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null)
                return View("Error404");

            Genre genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == Id && g.IsDeleted == false);

            if (genre == null)
                return View("Error404");

            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? Id, Genre genre)
        {
            if (Id == null)
                return View("Error404");

            if (!ModelState.IsValid)
            {
                return View(genre);
            }

            if (await _context.Genres.AnyAsync(g => g.Id != Id && g.Name.ToLower() == genre.Name.ToLower() && g.IsDeleted == false))
            {
                ModelState.AddModelError("Name", "Daxil Etdiyniz Adda Artiq Movcuddur");
                return View(genre);
            }

            Genre existedgenre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == Id);

            if (genre == null)
                return View("Error404");

            existedgenre.Name = genre.Name;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return View("Error404");

            Genre genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == Id && g.IsDeleted == false);

            if (genre == null)
                return View("Error404");

            genre.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
