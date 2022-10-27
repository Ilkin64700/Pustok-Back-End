using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pustok.Models
{
    public class Genre
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Genrenin Adi Mutleq Daxil Edilmelidir")]
        [StringLength(255,ErrorMessage ="Genrenin Adnin Uzunlugu Maksimum 255 Simvol Ola Biler")]
        [DataType(DataType.Text)]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
