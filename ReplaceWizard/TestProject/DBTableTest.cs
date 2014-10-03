using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReplaceWizard;

namespace TestProject
{
    class DBTableTest
    {
        [TestMethod]
        public void CreateAlteringScriptTest()
        {
            string alteration = "[dsFeriado] [varchar] (100) COLLATE Latin1_General_CI_AI NULL";

            DBTable table = new DBTable("TableName", "");


            //string script = table.CreateAlteringScript(new string[] { alteration }, new int[] { });

        }
    }
}
