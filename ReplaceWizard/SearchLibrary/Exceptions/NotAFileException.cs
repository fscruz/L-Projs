using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLibrary.Exceptions
{
    class NotAFileException : Exception
    {
        public NotAFileException(string msg, Exception innerException) : base(msg, innerException)
        { }
    }
}
