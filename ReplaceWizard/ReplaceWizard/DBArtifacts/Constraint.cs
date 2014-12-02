using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplaceWizard.DBArtifacts
{
    class Constraint : ITableComponent
    {
        private string scriptLine;

        public Constraint(string scriptLine)
        {
            // TODO: Complete member initialization
            this.scriptLine = scriptLine;
        }

        public DBTable Parent
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ComponentType Type
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        string ITableComponent.scriptLine
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public DBTable FindParent()
        {
            throw new NotImplementedException();
        }
    }
}
