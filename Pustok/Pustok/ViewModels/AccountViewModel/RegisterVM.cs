using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pustok.ViewModels.AccountViewModel
{
    public class RegisterVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string SurName { get; set; }
        [Required]
        public string FatherName { get; set; }
        public byte Age { get; set; }
        public string UserName { get; set; }
        [EmailAddress,DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,DataType(DataType.Password),Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
