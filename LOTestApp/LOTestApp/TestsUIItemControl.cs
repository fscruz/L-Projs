using LOSystemTests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LOTestApp
{
    class TestsUIItemControl
    {
        private ITestPrototype _test;

        private long? _time = null;

        private string _testResult = null;

        public string TimeTaken
        {
            get
            {
                if (_time == null)
                {
                    //_time = CheckTestTime();
                    return "";
                }
                double timeInSeconds = (double)_time / 1000;

                return timeInSeconds.ToString();
            }
        }

        public string TestName
        {
            get
            {
                return _test.DisplayName;
            }
        }

        public string TestResult
        {
            get
            {
                if (_testResult == null)
                {
                    //_testResult = MakeTest();
                    return "";
                }

                return _testResult;
            }
        }


        private long CheckTestTime()
        {
            Task<long> timeTask = TimedTestAsync();

            long time = timeTask.Result;

            timeTask.Wait();

            return time;
        }

        public async Task<long> TimedTestAsync()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            _testResult = await MakeTest();
            watch.Stop();

            return watch.ElapsedMilliseconds;
        }

        public async Task<string> MakeTest()
        {
            Task<string> testTask = new Task<string>(_test.Test);
            try
            {
                _testResult = await testTask;
            }
            catch (Exception ex)
            {
                Handler.ExceptionCollector(ex, TestName);

                throw new InsideTestException("An error occurred while executing" + TestName);
            }
            return _testResult;
        }
         
        public TestsUIItemControl(ITestPrototype test)
        {
            _test = test;
        }
    }
}
