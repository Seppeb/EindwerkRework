using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models
{
    public class Bericht
    {

        //properties
        public int Id { get; set; }
        public string Titel { get; set; }
        public string Inhoud { get; set; }
        public DateTime StartDate { get; set; }
        public bool BehandelaarBericht { get; set; }

        // relaties
        // met aanvraag
        public int AanvraagId { get; set; }
        [ForeignKey("AanvraagId")]
        public virtual Aanvraag Aanvraag { get; set; }

        // relatie
        // met user die bericht plaatst
        public string applicationuserId { get; set; }
        public virtual ApplicationUser applicationuser { get; set; }

    }
}
