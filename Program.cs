using Common.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialXmlLoader
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new PartialXmlLoad("xml/rsMap.xml");
            test.onReady = () => {
                Console.WriteLine("xml is ready ");

            };
            test.Load();

            Console.ReadKey();
        }
    }
}
