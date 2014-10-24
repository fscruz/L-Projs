using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;

namespace ReplaceWizard.DBArtifacts
{
    class DBView :IDBArtifact
    {
        public string BaseScript { get; set; }


        public string Name { get; set; }

        public string Type { get; private set; }

        public DBView()
        {
            Type = "View";
        }

        public string CreateAlteringScript(IEnumerable<string> listOfAlterations, IEnumerable<int> listOfIndexes)
        {
            throw new NotImplementedException();
        }

        public string CreateAlteringScript(string oldText, string newText)
        {
            int baseScriptStartIndex = this.BaseScript.ToLower().IndexOf("create");
            string baseScriptCorrected = this.BaseScript.Substring(baseScriptStartIndex, this.BaseScript.Length - baseScriptStartIndex);
            baseScriptCorrected = baseScriptCorrected.Replace(oldText, newText);

            string alteringScript = Scripts.AddExecConditionalDrop(this.Name, "VIEW");

            alteringScript += "\r\n";

            if (baseScriptCorrected.Contains("XML"))
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
