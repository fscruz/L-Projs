using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplaceWizard.DBArtifacts
{
    interface ITableComponent 
    {

        internal DBTable Parent { get; set; }

        internal ComponentType Type { get; set; }

        string scriptLine { get; set; }

        DBTable FindParent();

                
    }

    internal enum ComponentType
    {
        FullTextIndex,
        NCIndex,
        Column,
        Constraint
    }
}
