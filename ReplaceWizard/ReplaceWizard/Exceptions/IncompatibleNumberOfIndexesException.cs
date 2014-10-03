using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplaceWizard.Exceptions
{
    class IncompatibleNumberOfIndexesException : Exception
    {
        public IncompatibleNumberOfIndexesException(string msg) : base(msg)
        { }
    }
}
