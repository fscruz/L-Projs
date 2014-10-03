using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchLibrary
{
    class MultiThreadHelper
    {
        int _totalTasks;
        int _completedTasks;

        public int CompletedTasks
        {
            get { return _completedTasks; }
            set { _completedTasks = value; }
        }

        public MultiThreadHelper(int total)
        {
            _totalTasks = total;

            _completedTasks = 0;
        }

        public MultiThreadHelper()
        {
            _totalTasks = 0;
            _completedTasks = 0;
        }

        public double Percentage
        {
            get
            {
                if (_totalTasks != 0)
                {
                    return _completedTasks / _totalTasks;
                }
                else
                {
                    return 0;
                }
            }
        }
        
    }
}
