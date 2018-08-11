using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Paswoord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "bevestig paswoord")]
        [Compare("Password", ErrorMessage = "De paswoorden zijn niet gelijk")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Voornaam")]
        public string Voornaam { get; set; }

        [Required]
        [Display(Name = "Klant")]
        public string Klant { get; set; }

        [Required]
        [Display(Name = "Achternaam")]
        public string Achternaam { get; set; }

    }
}
