using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models
{
    public class Status
    {

        public int Id { get; set; }
        public string Naam { get; set; }
        public string Omschrijving { get; set; }

        //relatie
        //met aanvraag
        public ICollection<Aanvraag> Aanvragen { get; set; }

    }
}
