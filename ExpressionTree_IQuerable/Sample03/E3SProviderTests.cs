using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample03.E3SClient.Entities;
using Sample03.E3SClient;
using System.Configuration;
using System.Linq;

namespace Sample03
{
    [TestClass]
    public class E3SProviderTests
    {

        [TestMethod]
        public void InverseOrder()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            Console.WriteLine("Inverse order of params:");
            foreach (var emp in employees.Where(e => "EPBYMINW4646" == e.workstation))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [TestMethod]
        public void ExtendFilterWithAnd()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            Console.WriteLine("Extend E3S client with AND:");
            foreach (var emp in employees.Where(e => e.workstation.Contains("EPBYMI") && e.lastname.StartsWith("Sa")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }

        [TestMethod]
        public void StartsWithTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
            Console.WriteLine();

            Console.WriteLine("Name starts with 'Ca':");
            foreach (var emp in employees.Where(e => e.nativename.StartsWith("Са")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.birthday);
            }

        }

        [TestMethod]
        public void EndsWithTest()
        {
            var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

            Console.WriteLine("Workstation Ends With 46");
            foreach (var emp in employees.Where(e => e.workstation.EndsWith("46")))
            {
                Console.WriteLine("{0} {1}", emp.nativename, emp.startworkdate);
            }
        }
    }
}
