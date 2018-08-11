using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "de {0} moet tenminste {2} en maximum {1} karakters lang zijn", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "nieuwe paswoord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "bevestig nieuwe paswoord")]
        [Compare("NewPassword", ErrorMessage = "paswoord is niet hetzelfste als vorige")]
        public string ConfirmPassword { get; set; }

        public string StatusMessage { get; set; }
    }
}
