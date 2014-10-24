using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;
using ReplaceWizard.Exceptions;

namespace ReplaceWizard.DBArtifacts
{
    class DBProcedure : IDBArtifact
    {
        public string BaseScript { get; set; }

        public string Name { get; set; }

        public string Type { get; private set; }

        public DBProcedure()
        {
            Type = "PROC";
        }
        
        // Esta generalização não presta!!!
        //public string CreateAlteringScript(IEnumerable<string> listOfAlterations, IEnumerable<int> listOfIndexes)
        //{
        //    string[] alterations = listOfAlterations.ToArray();
        //    List<string> scriptPerLine;
        //    string alteringScript = Scripts.ExecConditionalDrop(this.Name, "PROC");
        //    string aux = this.BaseScript;

        //    scriptPerLine = aux.Split('\r\n').ToList();

        //    if(listOfAlterations.Count() != listOfIndexes.Count())
        //    {
        //        throw new IncompatibleNumberOfIndexesException("The number of elements in listOfIndexes does not match that of listOfAlterations");
        //    }

        //    alteringScript += "\r\n";

        //    for (int i = 0; i < alterations.Count(); i++)
        //    {
        //        string currentAlteration = alterations[i];
        //        int currentStartIndex = listOfIndexes.ToArray()[i];

        //        // Remove line that needs to be altered from base script

        //    }

        //    throw new NotImplementedException();
        //}

        public string CreateAlteringScript(string oldText, string newText)
        {

            int baseScriptStartIndex = this.BaseScript.ToLower().IndexOf("create");

            string baseScriptCorrected = this.BaseScript.Substring(baseScriptStartIndex, this.BaseScript.Length - baseScriptStartIndex);

            // Changes script from old to new.
            baseScriptCorrected = baseScriptCorrected.Replace(oldText, newText);

            // Add initial conditional drop for the proc to ensure the proc can be created. 
            string alteringScript = Scripts.AddExecConditionalDrop(this.Name, "PROC");

            alteringScript += "\r\n";

            if(baseScriptCorrected.Contains("XML"))
            {
                alteringScript += Scripts.AddQuotedIdentifiers();
            }

            alteringScript += "\r\n";

            alteringScript += Scripts.AddExec(baseScriptCorrected);

            alteringScript = alteringScript.RemoveWord("GO", '\n', '\r');

            return alteringScript;           
        }



        
    }
}
