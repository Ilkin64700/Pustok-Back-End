using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Pustok.Models
{
    public class Slider
    {
        public int Id { get; set; }
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Title { get; set; }
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        [StringLength(255)]
        public string  Image { get; set; }
        [StringLength(255)]
        public string OriginalImageName { get; set; }
        [DataType(DataType.Currency)]
        public double Price { get; set; }
        public int Order { get; set; }
        public bool IsDelete { get; set; }
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile File { get; set; }

    }
}
