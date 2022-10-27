using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Pustok.Models
{
    public class Product
    {
        public int Id { get; set; }
        //[Required]
        //[StringLength(255)]
        //public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        //[Required]
        [StringLength(255)]
        public string MainImage { get; set; }
        //[Required]
        [NotMapped]
        public IFormFile MainImageFile { get; set; }
        //[Required]
        [StringLength(255)]
        public string HoverImage { get; set; }
        //[Required]
        [NotMapped]
        public IFormFile HoverImageFile { get; set; }
        [Required]
        //[Column("ProductPrice",TypeName ="money")]
        [Column(TypeName = "decimal(18,2)")]
        public double Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Nullable<double> DiscountPrice { get; set; }
        public bool IsFeature { get; set; }
        public bool IsArrival { get; set; }
        public bool IsMostView { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public Nullable<double> ExTax { get; set; }
        [StringLength(255)]
        public string Code { get; set; }
        public int Point { get; set; }
        public bool IsAvailability { get; set; }
        public int Star { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }
        //[Required]
        public Nullable<int> AuthorId { get; set; }
        //[Required]
        public Nullable<int> GenreId { get; set; }

        public virtual Author Author { get; set; }
        public virtual Genre Genre { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        [NotMapped]
        //[MaxLength(5)]
        public IFormFile[] ProductImagesFile { get; set; }
    }
}
