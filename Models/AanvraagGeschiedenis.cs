using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models
{
    public class AanvraagGeschiedenis
    {

        public int Id { get; set; }
        public string Actie { get; set; }
        public DateTime time { get; set; }
        public string Voornaam  { get; set; }
        public string Achternaam { get; set; }
        public string Status { get; set; }
 

        //relaties
        //met aanvraag
        public int AanvraagId { get; set; }
        [ForeignKey("AanvraagId")]
        public virtual Aanvraag Aanvraag { get; set; }

        //met user
        


    }
}
