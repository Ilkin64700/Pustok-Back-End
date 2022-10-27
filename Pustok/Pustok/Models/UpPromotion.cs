using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class UpPromotion
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Image { get; set; }
    }
}
