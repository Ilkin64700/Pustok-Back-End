using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pustok.Models;

namespace Pustok.ViewModels.Home
{
    public class HomeVm
    {
        public ICollection<Slider> Sliders { get; set; }
        public ICollection<UpPromotion> UpPromotions { get; set; }
        public ICollection<Product> Feature { get; set; }
        public ICollection<Product> Arrival { get; set; }
        public ICollection<Product> MostView { get; set; }

    }
}
