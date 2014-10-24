using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;
using System.IO;
using System.Threading;
using System.Diagnostics;
using ReplaceWizard;
using ReplaceWizard.Exceptions;

namespace ConsoleDebug
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> dirs = new List<string>();

            dirs.Add(@"C:\LawOffice\WorkingArea\DBPro\Template35\Tables");
            dirs.Add(@"C:\LawOffice\WorkingArea\DBPro\Template35\Functions");
            dirs.Add(@"C:\LawOffice\WorkingArea\DBPro\Template35\Views");
            dirs.Add(@"C:\LawOffice\WorkingArea\DBPro\Template35\Stored Procedures");

            string oldText = "SQL_Latin1_General_CP1_CI_AI";

            string newText = "Latin1_General_CI_AI";

            //string find = "SQL_Latin1";


            //string oldText = "XML";

            //string newText = "XML";


            List<Stream> filesWithText = new List<Stream>();
            // Find all files with the given text in it.
            foreach (string dir in dirs)
            {
                List<Stream> filesInDir = Worker.GetAllFiles(dir, true).ToList();

                filesWithText.AddRange(filesInDir.SingleSearch(oldText, false));
            }

            // Read all Text
            List<string> textsFromSearchResults = Worker.ReadAllFiles(filesWithText).ToList();

            // Close Streams
            foreach(Stream file in filesWithText)
            {
                file.Close();
            }

            List<IDBArtifact> dbObjs = new List<IDBArtifact>();

            
            // Get all db objects from the resulting scripts.
            // ATENTION: The next methods assume all scripts are generated in Windows (because of CRLF endings) and in a specific indented basis (e.g. with a column per line in table creation scripts):
            foreach(string currentText in textsFromSearchResults)
            {
                Type typeOfScript;
                try
                {
                    typeOfScript = DBArtifactFactory.GetDBTypeFromScript(currentText);
                }
                catch (NotADBArtifactException)
                {
                    continue;
                }

                IDBArtifact currentObj = DBArtifactFactory.New(typeOfScript);

                currentObj.Name = DBHelper.GetDBOName(currentText);
                currentObj.BaseScript = currentText;

                dbObjs.Add(currentObj);

                // And alter them, depending on type
                string alterScript = currentObj.CreateAlteringScript(oldText, newText);

                // then save to file using object type to distinguish the subdirectory.
                CreateScriptFile(alterScript, currentObj.Name, currentObj.Type);

            }


            //foreach(string text in alteredTexts)
            //{
            //    Type typeOfScript = DBObjectFactory.GetDBTypeFromScript(text);

            //    if()
            //}


        }

        private static void CreateScriptFile(string alterScript, string objectName, string subDirectory)
        {
            // Prepare directory
            string directoryName = Path.Combine(Directory.GetCurrentDirectory(), subDirectory);

            // Add possible aditional subdirectories

            directoryName = Path.Combine(directoryName, "Collate");

            Directory.CreateDirectory(directoryName);
            
            // Write to file
            File.WriteAllText(Path.Combine(directoryName, objectName + ".sql"), alterScript, Encoding.UTF8);
        }
    }
}
