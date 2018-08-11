using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models
{
    public class Aanvraag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Gelieve een titel mee te geven aan de aanvraag")]
        public string Titel { get; set; }

        [Required(ErrorMessage = "Gelieve een korte omschrijving mee te geven aan de aanvraag")]
        public string Omschrijving { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Start")]
        [Required(ErrorMessage = "Gelieve een start datum te voorzien")]
        public DateTime StartDatum { get; set; }      
        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Required(ErrorMessage = "Gelieve een eind datum te voorzien")]
        [DisplayName("Einde")]
        public DateTime EindDatum { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Aanmaak aanvraag")]
        public DateTime? Aanmaakdatum { get; set; }

        [DisplayName("Link van de klant")]
        public string LinkVoorbeeldKlant { get; set; }

        [DisplayName("Link van de behandelaar")]
        public string LinkVoorbeeldBehandelaar { get; set; }

        public bool IsVraag { get; set; }

        //Relaties met users
               
        public string UserId { get; set; }
        public string BehandelaarId { get; set; }

        [ForeignKey("UserId")]
        [DisplayName("Gebruiker")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        //relatie met behandelaar
        [ForeignKey("BehandelaarId")]
        [DisplayName("Behandelaar")]
        public virtual ApplicationUser BehandelaarApplicationUser { get; set; }


        //relatie met Status
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public virtual Status Status { get; set; }
        //relatie met berichten
        public ICollection<Bericht> Berichten { get; set; }

        //erbij om relatie met behandelaar te maken meer op meer
        //[DisplayName("Behandelaars")]
        //public ICollection<AanvraagBehandelaar> AanvraagBehandelaars { get; set; }


    }
}
