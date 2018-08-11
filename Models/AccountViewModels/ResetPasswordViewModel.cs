using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "de {0} moet tenminste {2} en maximum {1} karakters lang zijn", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "bevestig paswoord")]
        [Compare("Password", ErrorMessage = "De paswoorden zijn niet gelijk")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
