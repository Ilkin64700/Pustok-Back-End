using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Extensions;
using Pustok.Helpers;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.OrderByDescending(s=>s.Id).Take(8).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            if (product.ProductImagesFile.Length > 5)
            {
                ModelState.AddModelError("ProductImagesFile", "Maksimum 5 Sekil Yukleye Bilersiniz");
                return View(product);
            }

            if (!ModelState.IsValid)
                return View(product);

            if (product.AuthorId != null && !await _context.Authors.AnyAsync(a=>a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author Mutleq Secilmelidi");
                return View(product);
            }

            if (product.GenreId != null && !await _context.Genres.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre Mutleq Secilmelidi");
                return View(product);
            }

            if (product.MainImageFile == null)
            {
                ModelState.AddModelError("MainImageFile", "Main Shekil Mutleq Secilmelidi");
                return View(product);

            }

            if (product.HoverImageFile == null)
            {
                ModelState.AddModelError("HoverImageFile", "Hover Shekil Mutleq Secilmelidi");
                return View(product);
            }

            if (!product.MainImageFile.CheckContentType("image"))
            {
                ModelState.AddModelError("MainImageFile", "MainImageFile tipini Duzgun Secin");
                return View(product);
            }

            if (product.MainImageFile.CheckLength(200))
            {
                ModelState.AddModelError("MainImageFile", "MainImageFile Uzunlugu Maksimum 200 kb Ola Biler");
                return View(product);
            }

            string filePath = Path.Combine(_env.WebRootPath, "image", "products");

            product.MainImage = await product.MainImageFile.SaveFileAsync(filePath);

            if (!product.HoverImageFile.CheckContentType("image"))
            {
                ModelState.AddModelError("HoverImageFile", "HoverImageFile tipini Duzgun Secin");
                return View(product);
            }

            if (product.HoverImageFile.CheckLength(200))
            {
                ModelState.AddModelError("HoverImageFile", "HoverImageFile Uzunlugu Maksimum 200 kb Ola Biler");
                return View(product);
            }

            product.HoverImage = await product.HoverImageFile.SaveFileAsync(filePath);

            if (product.ProductImagesFile.Length > 0)
            {
                List<ProductImage> productImages = new List<ProductImage>();

                foreach (IFormFile file in product.ProductImagesFile)
                {
                    if (!file.CheckContentType("image"))
                    {
                        ModelState.AddModelError("ProductImagesFile", $"{file.FileName} tipini Duzgun Secin");
                        return View(product);
                    }

                    if (file.CheckLength(200))
                    {
                        ModelState.AddModelError("ProductImagesFile", $"{file.FileName} Uzunlugu Maksimum 200 kb Ola Biler");
                        return View(product);
                    }

                    ProductImage productImage = new ProductImage
                    {
                        Name = await file.SaveFileAsync(filePath)
                    };

                    productImages.Add(productImage);
                }

                product.ProductImages = productImages;
            }

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<IActionResult> Update(int? Id)
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            if (Id == null)
                return NotFound();

            Product product = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == Id);

            if (product == null)
                return NotFound();

            return View(product);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int? Id, Product product)
        {
            ViewBag.Authors = await _context.Authors.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            if (Id == null)
                return NotFound();

            Product dbProduct = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == Id);

            if (dbProduct == null)
                return NotFound();

            if (!ModelState.IsValid)
                return View(dbProduct);

            if (product.AuthorId != null && !await _context.Authors.AnyAsync(a => a.Id == product.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "Author Mutleq Secilmelidi");
                return View(product);
            }

            if (product.GenreId != null && !await _context.Genres.AnyAsync(a => a.Id == product.GenreId))
            {
                ModelState.AddModelError("GenreId", "Genre Mutleq Secilmelidi");
                return View(product);
            }

            int canUpload = 5 - dbProduct.ProductImages.Count();

            if (product.ProductImagesFile != null && canUpload < product.ProductImagesFile.Length)
            {
                ModelState.AddModelError("ProductImagesFile", $"Maksumum {canUpload} Shekil Upload Ede Bilersinizi !!!!!!!");
                return View(dbProduct);
            }

            string filePath = Path.Combine(_env.WebRootPath, "image", "products");

            if (product.MainImageFile != null)
            {

                if (!product.MainImageFile.CheckContentType("image"))
                {
                    ModelState.AddModelError("MainImageFile", "MainImageFile tipini Duzgun Secin");
                    return View(dbProduct);
                }

                if (product.MainImageFile.CheckLength(200))
                {
                    ModelState.AddModelError("MainImageFile", "MainImageFile Uzunlugu Maksimum 200 kb Ola Biler");
                    return View(dbProduct);
                }

                Helper.DeleteFile(filePath, dbProduct.MainImage);

                dbProduct.MainImage = await product.MainImageFile.SaveFileAsync(filePath);

            }

            if (product.HoverImageFile != null)
            {
                if (!product.HoverImageFile.CheckContentType("image"))
                {
                    ModelState.AddModelError("HoverImageFile", "HoverImageFile tipini Duzgun Secin");
                    return View(product);
                }

                if (product.HoverImageFile.CheckLength(200))
                {
                    ModelState.AddModelError("HoverImageFile", "HoverImageFile Uzunlugu Maksimum 200 kb Ola Biler");
                    return View(product);
                }

                Helper.DeleteFile(filePath, dbProduct.HoverImage);

                dbProduct.HoverImage = await product.MainImageFile.SaveFileAsync(filePath);
            }

            if (product.ProductImagesFile.Length > 0)
            {
                foreach (IFormFile file in product.ProductImagesFile)
                {
                    if (!file.CheckContentType("image"))
                    {
                        ModelState.AddModelError("ProductImagesFile", $"{file.FileName} tipini Duzgun Secin");
                        return View(product);
                    }

                    if (file.CheckLength(200))
                    {
                        ModelState.AddModelError("ProductImagesFile", $"{file.FileName} Uzunlugu Maksimum 200 kb Ola Biler");
                        return View(product);
                    }

                    dbProduct.ProductImages.Add(new ProductImage
                    {
                        Name = await file.SaveFileAsync(filePath)
                    });
                }
            }

            dbProduct.Title = product.Title;
            dbProduct.Price = product.Price;
            dbProduct.Point = product.Point;
            dbProduct.Star = product.Star;
            dbProduct.IsMostView = product.IsMostView;
            dbProduct.IsFeature = product.IsFeature;
            dbProduct.IsAvailability = product.IsAvailability;
            dbProduct.IsArrival = product.IsArrival;
            dbProduct.GenreId = product.GenreId;
            dbProduct.ExTax = product.ExTax;
            dbProduct.DiscountPrice = product.DiscountPrice;
            dbProduct.Description = product.Description;
            dbProduct.Code = product.Code;
            dbProduct.AuthorId = product.AuthorId;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProductImage(int? Id)
        {
            if (Id == null)
                return NotFound();

            ProductImage productImage = await _context.ProductImages.FirstOrDefaultAsync(p => p.Id == Id);
            if (productImage == null)
                return NotFound();

            string filePath = Path.Combine(_env.WebRootPath, "image", "products");

            Helper.DeleteFile(filePath, productImage.Name);

            int productId = productImage.ProductId;

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();


            return RedirectToAction("Update",new { Id=productId});
        }
    }
}
