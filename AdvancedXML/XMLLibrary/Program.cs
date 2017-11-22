using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XMLLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.xml");
            ValidateFile(inputXml);
            TrasformToRss(inputXml);
            TrasformToHtml(inputXml);
            Console.ReadKey();
        }

        public static void ValidateFile(string inputXml)
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            settings.Schemas.Add("http://library.by/catalog", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.xsd"));
            settings.ValidationEventHandler +=
                delegate (object sender, ValidationEventArgs e)
                {
                    Console.WriteLine("[{0}:{1}] {2}", e.Exception.LineNumber, e.Exception.LinePosition, e.Message);
                };

            settings.ValidationFlags = settings.ValidationFlags | XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationType = ValidationType.Schema;
            XmlReader reader = XmlReader.Create(inputXml, settings);

            while (reader.Read()) ;
        }

        public static void TrasformToRss(string inputXml)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            XPathDocument xpathdocument = new XPathDocument(inputXml);
            XmlTextWriter writer = new XmlTextWriter("result.txt", Encoding.Unicode);
            xsl.Load("transformRSS.xslt");
            writer.Formatting = Formatting.Indented;
            xsl.Transform(xpathdocument, null, writer, null);
        }

        public static void TrasformToHtml(string inputXml)
        {
            XslCompiledTransform xsl = new XslCompiledTransform();
            XPathDocument xpathdocument = new XPathDocument(inputXml);
            XmlTextWriter writer = new XmlTextWriter("result.html", Encoding.Unicode);
            XsltSettings settings = new XsltSettings(false, true);
            xsl.Load("transformToHTML.xslt", settings, new XmlUrlResolver());
            writer.Formatting = Formatting.Indented;
            xsl.Transform(xpathdocument, null, writer, null);
        }

    }
}
