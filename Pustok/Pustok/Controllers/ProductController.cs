using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pustok.ViewModels.ProductViewModel;

namespace Pustok.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDetail(int? Id)
        {
            if (Id == null)
                return NotFound();

            //if (!await _context.Products.AnyAsync(p=>p.Id == Id))
            //    return NotFound();

            Product product = await _context.Products
                .Include(p=>p.Author)
                .Include(p=>p.Genre)
                .Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (product == null)
                return NotFound();

            return PartialView("_ProductDetailPartial", product);

            //return Json(product);
        }

        public async Task<IActionResult> AddBasket(int? Id)
        {
            if (Id == null)
                return NotFound();

            Product product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == Id);

            if (product == null)
                return NotFound();

            string strBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> products = null;

            if (strBasket == null)
            {
                products = new List<BasketVM>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(strBasket);
            }

            BasketVM basketVM = new BasketVM
            {
                Id = (int)Id,
                Title = product.Title,
                MainImage = product.MainImage,
                Price = product.DiscountPrice != null ? (double)product.DiscountPrice : product.Price,
                Count = 1
            };

            if (products.Any(p=>p.Id == Id))
            {
                products.FirstOrDefault(p => p.Id == Id).Count += 1;
            }
            else
            {
                products.Add(basketVM);
            }

            string strProduct = JsonConvert.SerializeObject(products);

            HttpContext.Response.Cookies.Append("basket", strProduct, new CookieOptions { MaxAge = TimeSpan.FromMinutes(10) });


            //List<Product> products = new List<Product>();
            //products.Add(product);



            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ShowBasket()
        {
            string strBasket = HttpContext.Request.Cookies["basket"];

            List<BasketVM> products = null;

            if (strBasket == null)
            {
                products = new List<BasketVM>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(strBasket);
            }
            return Json(products);
        }
    }
}
