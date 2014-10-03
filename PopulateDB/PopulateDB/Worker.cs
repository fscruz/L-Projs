using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopulateDB
{
    class Worker
    {
        TesteDBEntities _dbs;

        public Worker()
        {
            _dbs = new TesteDBEntities();
        }

        public void Populate()
        {
            _dbs.testetlb.AddRange(GenerateData());

            _dbs.SaveChanges();
        }

        public List<testetlb> GenerateData()
        {
            var data = new List<testetlb>();
            var codes = new List<int>();

            for(int i = 0; i < (2^256); i++)
            {
                codes.Add(i);

                if(codes.Count >= 50)
                {
                    string varcharEntry = "";
                    try
                    {
                        varcharEntry = GenerateStringLine(codes.ToArray());
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Codigo Hex invalido! " + ex.Message);
                        continue;
                    }
                    testetlb entry = new testetlb();
                    entry.string1 = varcharEntry;
                    entry.string2 = varcharEntry;

                    data.Add(entry);
                    codes.Clear();
                }
            }

            return data;
        }

        public string GenerateStringLine(int[] hexCodes)
        {
            string result = "";
            foreach(int code in hexCodes)
            {
                result += char.ConvertFromUtf32(code).ToString();
            }

            return result;
        }
        

    }
}
