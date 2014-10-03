using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;


namespace ReplaceWizard
{
    public static class DBHelper
    {

        public static string GetDBOName(string baseScript)
        {
            string scriptCaseInsensitive = baseScript.ToLower();
            string aux = baseScript;


            int createCmdIndex = scriptCaseInsensitive.Find("create");
            aux = baseScript.Remove(0, createCmdIndex);

            int startOfName = aux.IndexOf('[');

            string dbObjectName = aux.Remove(0, startOfName);

            int endOfName = dbObjectName.IndexOf(']');
            
            // The object name will come in the format [<databaseowneruser>].[<objectname>] or something similar
            //so search for the name until the last dot
            int lastDotPossiblePosition = endOfName + 1;

            while(dbObjectName[lastDotPossiblePosition] == '.' )
            {
                endOfName = dbObjectName.IndexOf(']', lastDotPossiblePosition);

                lastDotPossiblePosition = endOfName + 1;
            }

            dbObjectName = dbObjectName.Substring(0, endOfName + 1);

            return dbObjectName;

        }
    }
}
