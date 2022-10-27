using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Pustok.Models;
using Pustok.ViewModels.AccountViewModel;
using Pustok.ViewModels.ProductViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Services
{
    public class LayoutService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AppUser> _userManager;

        public LayoutService(IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
        {
            _httpContext = httpContext;
            _userManager = userManager;
        }

        public async Task<AppUserVM> GetUser()
        {
            AppUserVM appUserVM = null;
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                AppUser appUser = await _userManager.FindByNameAsync(_httpContext.HttpContext.User.Identity.Name);

                appUserVM = new AppUserVM
                {
                    Name = appUser.Name,
                    SurName = appUser.SurName
                };
            }

            return appUserVM;
        }

        public ICollection<BasketVM> GetBasket()
        {
            string strBasket = _httpContext.HttpContext.Request.Cookies["basket"];

            List<BasketVM> products = null;

            if (strBasket == null)
            {
                products = new List<BasketVM>();
            }
            else
            {
                products = JsonConvert.DeserializeObject<List<BasketVM>>(strBasket);
            }
            return products;
        }
    }
}
