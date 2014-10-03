using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLibrary.Exceptions
{
    /// <summary>
    /// Occurs when quotes in the text are not paired.
    /// </summary>
    class NotFullyQuotedException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public NotFullyQuotedException(string msg)
            : base(msg)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public NotFullyQuotedException(string msg, Exception innerException)
            : base(msg, innerException)
        { }
    }
}
