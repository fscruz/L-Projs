using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplaceWizard.DBArtifacts
{
    class FTIndex : ITableComponent
    {
        private string scriptLine;

        public FTIndex(string scriptLine)
        {
            // TODO: Complete member initialization
            this.scriptLine = scriptLine;
        }
    }
}
