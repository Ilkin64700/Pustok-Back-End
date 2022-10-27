using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.Models
{
    public class Setting
    {
        public int Id { get; set; }
        [StringLength(255)]
        public string Logo { get; set; }
        [StringLength(255)]
        public string SupportPhone { get; set; }
        [StringLength(255)]
        public string Address { get; set; }
        [StringLength(255)]
        public string Phone { get; set; }
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(255)]
        public string FacebookUrl { get; set; }
        [StringLength(255)]
        public string TwitterUrl { get; set; }
        [StringLength(255)]
        public string GoogleUrl { get; set; }
        [StringLength(255)]
        public string YoutubeUrl { get; set; }
    }
}
