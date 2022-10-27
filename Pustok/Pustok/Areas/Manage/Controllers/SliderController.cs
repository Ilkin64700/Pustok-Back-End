using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Pustok.Extensions;
using Pustok.Helpers;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SliderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Sliders.Where(s=>s.IsDelete == false).ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Slider slider)
        {
            if (_context.Sliders.Where(s=>s.IsDelete == false).Count() >= 5)
                return NotFound();

            if (!ModelState.IsValid)
                return View(slider);

            if (!slider.File.CheckContentType("image"))
            {
                ModelState.AddModelError("File", "Duzgun File Secin");
                return View(slider);
            }

            if (slider.File.CheckLength(300))
            {
                ModelState.AddModelError("File", "Seklin Olcusu Maksimum 300 kb ola Biler");
                return View(slider);
            }

            slider.OriginalImageName = slider.File.FileName;

            string filepath = Path.Combine(_env.WebRootPath, "image", "bg-images");

            slider.Image = await slider.File.SaveFileAsync(filepath);

            await _context.Sliders.AddAsync(slider);
            await _context.SaveChangesAsync();
                
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? Id)
        {
            if (Id == null)
                return NotFound();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);

            if (slider == null)
                return NotFound();

            return View(slider);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null)
                return NotFound();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);

            if (slider == null)
                return NotFound();

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? Id, Slider slider)
        {
            if (Id == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(slider);
            }

            Slider dbSlider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);

            if (dbSlider == null)
                return NotFound();

            if (slider.File != null)
            {
                if (!slider.File.CheckContentType("image"))
                {
                    ModelState.AddModelError("File", "Duzgun File Secin");
                    return View(slider);
                }

                if (slider.File.CheckLength(300))
                {
                    ModelState.AddModelError("File", "Seklin Olcusu Maksimum 300 kb ola Biler");
                    return View(slider);
                }

                string filepath = Path.Combine(_env.WebRootPath, "image", "bg-images");

                Helper.DeleteFile(filepath, dbSlider.Image);

                dbSlider.OriginalImageName = slider.File.FileName;

                dbSlider.Image = await slider.File.SaveFileAsync(filepath);
            }

            dbSlider.Order = slider.Order;
            dbSlider.Price = slider.Price;
            dbSlider.Title = slider.Title;
            dbSlider.Description = slider.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return NotFound();

            Slider slider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == Id && s.IsDelete == false);

            if (slider == null)
                return NotFound();

            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, Slider slider)
        {
            if (Id == null)
                return NotFound();

            Slider Deletedslider = await _context.Sliders.FirstOrDefaultAsync(s => s.Id == Id);

            if (slider == null)
                return NotFound();

            string path = Path.Combine(_env.WebRootPath, "image", "bg-images");

            Helper.DeleteFile(path, Deletedslider.Image);

            Deletedslider.IsDelete = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
