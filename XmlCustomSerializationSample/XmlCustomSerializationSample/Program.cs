using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace XmlCustomSerializationSample
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // https://stackoverflow.com/questions/34648136/xml-serialization-deserialization-html-entities-c-sharp-net
            Console.WriteLine("Html Entityの変換を防ぐ");
            
            var text =
                @"<?xml version=""1.0"" encoding=""UTF-8""?>
<GroupFile>
    <Group id=""10"" desc=""Description"">
        <Member id=""117"">&#x00B0;</Member>
    </Group>    
</GroupFile>";

            Console.WriteLine("Read XML ...");
            using (var stream = new StringReader(text))
            {
                // xml reader setting.
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings()
                {
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                };

                // xml reader create.
                using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(GroupFile));
                    var grp = (GroupFile) xmlSerializer.Deserialize(xmlReader);

                    var mem = grp.Groups[0].Members[0];
                    // HexValue...
                    Console.WriteLine($"GroupFile HexValue={mem.HexValue} Value={mem.Value}");
                    
                    Console.WriteLine("");
                    Console.WriteLine("Write XML ...");
                    Console.WriteLine($"{WriteXml(grp)}");
                }
            }
            Console.WriteLine($"Push any key...");
            Console.ReadKey();
        }

        private static string WriteXml(GroupFile value)
        {
            var settings = new XmlWriterSettings
            {
//                NewLineChars = "\n",
//                OmitXmlDeclaration = true, //なぜかUTF-8が16になってしまうので自分で
                Indent = true,
                IndentChars = "\t",
//                NamespaceHandling = NamespaceHandling.OmitDuplicates,
//                DoNotEscapeUriAttributes = true,
            };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                var serializer = new XmlSerializer(typeof(GroupFile));
                serializer.Serialize(writer, value, null);
                return stream.ToString();
            }

        }
    }
}