using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationRequestIt.Utility
{
    public class SD
    {
        public const string AdminEndUser = "Admin";
        public const string BehandelaarEndUser = "Behandelaar";
        public const string CustomerEndUser = "Customer";
        public const string AdminBehandelaar = AdminEndUser +","+ BehandelaarEndUser;
    }
}
