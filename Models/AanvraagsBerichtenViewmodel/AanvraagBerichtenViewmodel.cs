using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Models.AanvraagsBerichtenViewmodel
{
    public class AanvraagBerichtenViewmodel
    {
        public IList<Bericht> berichten { get; set; }
        public Aanvraag aanvraag { get; set; }
        public Bericht bericht { get; set; }
    }
}
