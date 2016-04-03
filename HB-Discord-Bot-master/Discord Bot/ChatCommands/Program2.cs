using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApplication2
{
    class Program2
    {
        static void Main(string[] args)
        {
            XElement Voting = XElement.Load("../LocalFiles/Voting.xml");
            XElement Name = new XElement("Name");
        }
    }
}
