using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace ApplicationRequestIt.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Voornaam { get; set; }
        public string Achternaam { get; set; }
        public string Klant { get; set; }
        public bool? isEnabled { get; set; }

        [DisplayName("Admin")]
        public bool isAdmin { get; set; }
        [DisplayName("Behandelaar")]
        public bool isBehandelaar { get; set; }

        public ICollection<Aanvraag> CustomerAanvragen { get; set; }
        public ICollection<Aanvraag> BehandelaarAanvragen { get; set; }
        public ICollection<Bericht> UserBerichten { get; set; }

        //extra om relatie te maken met aanvragen en behandelaren meer op meer
        //public List<AanvraagBehandelaar> AanvraagBehandelaars { get; set; }

    }
}
