using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplaceWizard.Exceptions
{
    public class NotADBArtifactException : Exception
    {
        public NotADBArtifactException(string msg) : base(msg)
        { }

        public NotADBArtifactException()
            : base()
        { }
    }
}
