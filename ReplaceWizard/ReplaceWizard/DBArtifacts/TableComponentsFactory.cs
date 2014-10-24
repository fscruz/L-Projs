using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard.DBArtifacts
{
    static class TableComponentsFactory
    {
        public static ITableComponent CreateInstanceOf(Type dbObjType)
        {
            if(!(typeof(ITableComponent).IsAssignableFrom(dbObjType)))
            {
                throw new Exceptions.NotADBArtifactException();
            }
            // Creating through parameterless constructor.
            return (ITableComponent)Activator.CreateInstance(dbObjType);
        }

        /// <summary>
        /// Gets the type of component the given script creates.
        /// </summary>
        /// <param name="script">Script line to analyse.</param>
        /// <returns></returns>
        public static Type GetComponentTypeFromScript(string scriptLine)
        {
            string scriptCaseInsensitive = scriptLine.ToLower();


            //if (!scriptCaseInsensitive.Contains("create") && !scriptCaseInsensitive.Contains("alter"))
            //{
            //    throw new ArgumentException("Argument scriptLine is not a TableComponent creation sql command");
            //}

            if (scriptCaseInsensitive.Contains("constraint"))
            {
                return typeof(Constraint);
            }

            if (scriptCaseInsensitive.Contains("fulltext"))
            {
                return typeof(FTIndex);
            }

            if (scriptCaseInsensitive.Contains("nonclustered"))
            {
                return typeof(NCIndex);
            }

            // Columns have no 
            return typeof(Column);
        }
            
    }
}
