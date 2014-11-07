using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LOTestApp
{
    class InsideTestException : Exception
    {

        public InsideTestException(string msg) : base(msg)
        {
        }

    }
}
