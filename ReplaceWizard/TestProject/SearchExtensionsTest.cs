using Microsoft.VisualStudio.TestTools.UnitTesting;
using SearchLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
    class SearchExtensionsTest
    {
        // Not a real test method (NORTM).
        [TestMethod]
        public void SingleSearchAsyncTest()
        {
            List<Stream> lookHere = new List<Stream>();

            for (int i = 0; i < 10000; i++)
            {
                using (StreamWriter writer = new StreamWriter(String.Format(@"C:\LawOffice\WorkingArea\Projects\ReplaceWizard\ConsoleDebug\bin\Debug\Test{0}.txt", i), false))
                {
                    writer.WriteLine(String.Format("Numero:{0}", i));
                }

                FileStream file = new FileStream(String.Format(@"C:\LawOffice\WorkingArea\Projects\ReplaceWizard\ConsoleDebug\bin\Debug\Test{0}.txt", i), FileMode.Open, FileAccess.ReadWrite);
                lookHere.Add(file);
            }

            Stopwatch watch = Stopwatch.StartNew();

            watch.Start();

            Task<IEnumerable<Stream>> teste = lookHere.SingleSearchAsync("1", false);

            List<Stream> result = teste.Result.ToList();

            int consecutiveEquals = 0;

            int lastCount = 0;

            while (consecutiveEquals < 10)
            {

                if (result.Count == lastCount)
                {
                    consecutiveEquals++;
                }
                else
                {
                    consecutiveEquals = 0;
                    Console.WriteLine("Count = " + result.Count.ToString() + "Time = " + watch.ElapsedMilliseconds.ToString());
                }

                lastCount = result.Count;

                Thread.Sleep(20);
            }

            watch.Stop();

            Console.WriteLine("Found: " + result.Count.ToString() + " Time: " + watch.ElapsedMilliseconds.ToString());

            foreach (Stream file in lookHere)
            {
                file.Close();
            }

        }
        
    }
}
