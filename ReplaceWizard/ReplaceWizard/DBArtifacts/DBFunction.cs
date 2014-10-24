using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;

namespace ReplaceWizard.DBArtifacts
{
    class DBFunction : IDBArtifact
    {
        public string BaseScript { get; set; }

        public string Name { get; set; }

        public string Type { get; private set; }

        public DBFunction()
        {
            Type = "Function";
        }

        public string CreateAlteringScript(string oldText, string newText)
        {
            int baseScriptStartIndex = this.BaseScript.ToLower().IndexOf("create");

            string baseScriptCorrected = this.BaseScript.Substring(baseScriptStartIndex, this.BaseScript.Length - baseScriptStartIndex);

            baseScriptCorrected = baseScriptCorrected.Replace(oldText, newText);

            string alteringScript = Scripts.AddExecConditionalDrop(this.Name, "PROC");

            alteringScript += "\r\n";

            if(baseScriptCorrected.Contains("XML"))
            {
                alteringScript += Scripts.AddQuotedIdentifiers();
            }

            alteringScript += "\r\n";

            alteringScript += Scripts.AddExec(baseScriptCorrected);

            //alteringScript = alteringScript.LastIndexOf("");

            // Removing all GO ocurrences, considering, as 'spacing', line endings, '\n' and '\r', and the default ' '
            alteringScript = alteringScript.RemoveWord("GO", '\n', '\r');

            return alteringScript;  
        }
    }
}
