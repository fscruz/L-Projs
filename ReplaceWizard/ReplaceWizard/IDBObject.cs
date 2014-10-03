using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard
{
    public interface IDBObject
    {
        String BaseScript { get; set; }

        String Name { get; set; }

        String Type { get; }

        String CreateAlteringScript(string oldText, string newText);
    }
}
