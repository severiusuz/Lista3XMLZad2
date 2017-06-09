using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Zad2.Models;

namespace Zad2
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var xmlName = "mojxml.xml";
            var xsdName = "mojxml.xsd";
            var fullXmlPath = Path.Combine(filePath, xmlName);
            var fullXsdPath = Path.Combine(filePath, xsdName);

            var person = new Person { Name = "Hubi", LastName = "Szatan", Age = 12 };

            SaveXMLToFile(fullXmlPath, person);
            ValidateXML(fullXmlPath, fullXsdPath);

            Console.ReadKey();
        }

        public static void ValidateXML(string xmlPath, string xsdPath)
        {
            XmlTextReader tr = new XmlTextReader(xmlPath);
            XmlValidatingReader vr = new XmlValidatingReader(tr);

            vr.Schemas.Add(null, xsdPath);
            vr.ValidationType = ValidationType.Schema;
            vr.ValidationEventHandler += new ValidationEventHandler(ValidationHandler);

            while (vr.Read())
            {
                PrintTypeInfo(vr);
                if (vr.NodeType == XmlNodeType.Element)
                {
                    while (vr.MoveToNextAttribute())
                        PrintTypeInfo(vr);
                }
            }
            Console.WriteLine("Validation finished");
        }

        public static void SaveXMLToFile(string path, Person person)
        {
            var xmlSerializer = new XmlSerializer(typeof(Person));

            using (var writer = new StreamWriter(path))
            {
                xmlSerializer.Serialize(writer, person);
            }
        }

        public static void PrintTypeInfo(XmlValidatingReader vr)
        {
            if (vr.SchemaType != null)
            {
                if (vr.SchemaType is XmlSchemaDatatype || vr.SchemaType is XmlSchemaSimpleType)
                {
                    object value = vr.ReadTypedValue();
                    Console.WriteLine("{0}({1},{2}):{3}", vr.NodeType, vr.Name, value.GetType().Name, value);
                }
                else if (vr.SchemaType is XmlSchemaComplexType)
                {
                    XmlSchemaComplexType sct = (XmlSchemaComplexType)vr.SchemaType;
                    Console.WriteLine("{0}({1},{2})", vr.NodeType, vr.Name, sct.Name);
                }
            }
        }

        public static void ValidationHandler(object sender, ValidationEventArgs args)
        {
            Console.WriteLine("***Validation error");
            Console.WriteLine("\tSeverity:{0}", args.Severity);
            Console.WriteLine("\tMessage:{0}", args.Message);
        }
    }
}
