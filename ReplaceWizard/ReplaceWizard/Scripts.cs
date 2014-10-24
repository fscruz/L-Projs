using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard
{
    /// <summary>
    /// Contains methods to help create SQL scripts
    /// </summary>
    public static class Scripts
    {
        public static string AddAlterColumn(string column, string tableName)
        {
            return "ALTER TABLE " + tableName + " ALTER COLUMN " + column + " \r\n";
        }

        public static string AddExecConditionalDrop(string objectName, string objType)
        {
            if(objectName.Contains('\'') || objectName.Contains(' '))
            {
                throw new ArgumentException("Argument objectName contains invalid characters.");
            }

            if (objType.Contains('\'') || objType.Contains(' '))
            {
                throw new ArgumentException("Argument objType contains invalid characters.");
            }

            return "EXEC dbo.sp_executesql @statement = N'IF EXISTS ( SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N''" + objectName + "'')) \r\n DROP " + objType + " ''" + objectName + "'' '\r\n";
        }

        public static string AddExec(string baseScript)
        {
            string scriptWithCorrectQuotes = baseScript.Replace("'", "''");

            return "EXEC dbo.sp_executesql @statement = N' \r\n" + scriptWithCorrectQuotes + " \r\n'";
        }

        public static string AddColumnExistCondition(string column, string tableName, bool exist)
        {
            string response = "IF ";
            if(!exist)
            {
                response += "NOT ";
            }
            response += "EXISTS ( SELECT 1 FROM SYSCOLUMNS WHERE  ID = OBJECT_ID('" + tableName + "') AND NAME = '" + column + "' ) \r\n";
            return response;
        }


        internal static string AddQuotedIdentifiers()
        {
            return "SET QUOTED_IDENTIFIER ON \r\n GO \r\n";
        }

        internal static string AddPrint(string message)
        {
            return "PRINT '" + message.Replace("\'", "\'\'") + "'";
        }
    }
    
}
