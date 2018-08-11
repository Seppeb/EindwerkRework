using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models
{
    public class AanvraagBehandelaar
    {

        //tussentabbel om meer op meer relatie te maken tussen behandelaars en aanvragen

        public int AanvraagId { get; set; }
        public Aanvraag Aanvraag { get; set; }

        public string BehandelaarId { get; set; }
        public ApplicationUser Behandelaar { get; set; }


    }
}
