using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "gelieve je gebruikersnaam te voorzien")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "gelieve je paswoord te voorzien")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "herinner me?")]
        public bool RememberMe { get; set; }
    }
}
