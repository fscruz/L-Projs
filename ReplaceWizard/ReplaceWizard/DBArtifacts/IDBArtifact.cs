using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard.DBArtifacts
{
    /// <summary>
    /// Represents a 'scriptfied' database artifact. 
    /// </summary>
    public interface IDBArtifact
    {
        /// <summary>
        /// Base creation script for the database artifact.
        /// </summary>
        String BaseScript { get; set; }

        /// <summary>
        /// Name of the artifact in the database.
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Type of current artifact in the database.
        /// </summary>
        String Type { get; }

        /// <summary>
        /// When implemented, creates a script that serves to alter the database representation of current object, replacing all ocurrences of a given text for another
        /// </summary>
        /// <param name="oldText"></param>
        /// <param name="newText"></param>
        /// <example>CreateAlteringScript("varchar(30)", "varchar(50)") will create a script that alters all parameters of type varchar(30) to type varchar(50) in current DBArtifact</example>
        /// <returns></returns>
        String CreateAlteringScript(string oldText, string newText);

        //IEnumerable<String> CreateAlteringScript()
    }
}
