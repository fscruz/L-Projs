using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SearchLibrary;

namespace ReplaceWizard.DBArtifacts
{
    public class DBTable : IDBArtifact
    {
        public String BaseScript { get; set; }

        public String Name { get; set; }

        public string Type { get; private set; }

        internal ITableComponent Components { get; set; }

        public DBTable(string name, string baseScript)
        {
            Name = name;
            BaseScript = baseScript;
            Type = "Table";
        }

        public DBTable()
        {
            Type = "Table";
        }


        public string CreateAlteringScript(string oldText, string newText)
        {

            string alteringScript = "--Script created to alter " + this.Name + "\r\n\r\n";

            // Scripts to alter tables must be done in a single-column basis
            // So, first, divide the Base script for table creation on each column
            List<string> scriptLines = this.BaseScript.Split(new char[] { '\r' , '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList(); 

            

            // then change all occurrences of oldText to newText
            List<string> listOfAlterations = new List<string>();

            foreach(string line in scriptLines)
            {
                // if line endsWith ',', remove it
                string trimmedLine = line.TrimEnd(' ', ',');

                if (trimmedLine.Contains(oldText))
                {
                    alteringScript += CreateSingleAlterationScript(trimmedLine.Replace(oldText, newText));
                }
            }

            return alteringScript;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listOfAlterations"></param>
        /// <param name="listOfIndexes"></param>
        /// <returns></returns>
        public string CreateAlteringScript(IEnumerable<string> listOfAlterations)
        {
            // For tables, altering scripts only need the list of alterations.

            string alteringScript = "--Script created to alter" + this.Name + "\r\n\r\n";

            //alteringScript += Scripts.

            foreach (string alteration in listOfAlterations)
            {
                alteringScript += CreateSingleAlterationScript(alteration);
            }
            return alteringScript;
        }

        private string CreateSingleAlterationScript(string alteration)
        {

            string alterationScript = "";

            AlterationType alterationType = GetAlterationType(alteration);

            switch (alterationType)
            {
                case AlterationType.Column:
                    alterationScript += CreateAlterColumnScript(alteration);
                    break;
                case AlterationType.Constraint:
                    alterationScript += CreateAlterConstraintScript(alteration);
                    break;
                case AlterationType.Index:
                    alterationScript += CreateAlterIndexScript(alteration);
                    break;
                default:
                    break;
            }

            alterationScript += "\r\n\r\n";

            return alterationScript;
        }

        private AlterationType GetAlterationType(string alteration)
        {
            if(alteration.Contains("CONSTRAINT"))
            {
                return AlterationType.Constraint;
            }
            else if(alteration.Contains("INDEX"))
            {
                return AlterationType.Index;
            }
            else if(alteration.Contains("FULLTEXT"))
            {
                return AlterationType.FullText;
            }
            else
            {
                return AlterationType.Column;
            }
        }


        public string CreateAlterColumnScript(string alteration)
        {
            string[] alterationValues = alteration.SplitToMinimun().ToArray();

            // The list of alterations for columns always come with the column name as the first word of the alteration. ( HARDCODING!!!!:'( )
            // Store column Name and remove from alteration line.
            string columnName = alterationValues[0];

            alteration = alteration.Replace(columnName, "");

            // Trim column name.
            columnName = columnName.TrimStart('[');
            columnName = columnName.TrimEnd(']');

            string cleanTableName = this.Name.Replace("[dbo].", "");
            cleanTableName = cleanTableName.TrimStart('[');
            cleanTableName = cleanTableName.TrimEnd(']');


            string alterColumnScript = Scripts.AddColumnExistCondition(columnName, this.Name, true);

            alterColumnScript += "BEGIN \r\n";
            alterColumnScript += Scripts.AddAlterColumn(columnName, cleanTableName) + " " + alteration + "\r\n";
            alterColumnScript += Scripts.AddPrint("Column " + columnName + " altered.");
            alterColumnScript += "\r\n";
            alterColumnScript += "END \r\n";

            return alterColumnScript;
        }

        private string CreateAlterIndexScript(string alteration)
        {
            throw new NotImplementedException();
        }

        private string CreateDropIndexScript(string Index)
        {
            throw new NotImplementedException();
        }

        private string CreateAlterConstraintScript(string alteration)
        {
            throw new NotImplementedException();
        }



        enum AlterationType
        {
            Column,
            Constraint,
            Index,
            FullText
        }


        
    }
}
