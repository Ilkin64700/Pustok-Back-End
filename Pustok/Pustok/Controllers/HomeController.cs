using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pustok.ViewModels.Home;
using Microsoft.AspNetCore.Http;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }
         
        public async Task<IActionResult> Index()
        {
            //HttpContext.Session.SetString("Test","Hello P221");
            //return Content(HttpContext.Session.GetString("Test"));

            //HttpContext.Response.Cookies.Append("P221", "Hello P221 Coockie", new CookieOptions { MaxAge = TimeSpan.FromMinutes(10) });

            //return Content(HttpContext.Request.Cookies["P221"]);

            return View(new HomeVm
            {
                Sliders = await _context.Sliders.Where(s=>s.IsDelete == false).ToListAsync(),
                UpPromotions = await _context.UpPromotions.ToListAsync(),
                Feature = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsFeature).OrderByDescending(p => p.Id).Take(8).ToListAsync(),
                Arrival = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsArrival).OrderByDescending(p => p.Id).Take(8).ToListAsync(),
                MostView = await _context.Products.Include(p => p.Author).Include(p => p.Genre).Where(p => p.IsMostView).OrderByDescending(p => p.Id).Take(8).ToListAsync()
            });
    
        }
    }
}
