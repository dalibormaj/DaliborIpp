using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sks365.Ippica.Common.Utility
{
    public static class XmlConverter
    {
        public static string Serialize<T>(T xmlObject)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, xmlObject);

                string output = Encoding.UTF8.GetString(xmlStream.ToArray());
                return output;
             }
        }
    }
}
