using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplaceWizard.DBArtifacts
{
    class Column : ITableComponent
    {
        internal string scriptLine;

        public Column(string scriptLine)
        {
            // TODO: Complete member initialization
            this.scriptLine = scriptLine;
        }
        public ComponentType Type
        {
            set { throw new NotImplementedException(); }
        }
    }
}
