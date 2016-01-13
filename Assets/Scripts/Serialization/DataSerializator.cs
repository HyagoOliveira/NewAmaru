using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Serialization
{
	public static class DataSerializator
	{
        public static void CreateXML<T>(string fileLocation, string fileName, T data)
        {
            CheckDirectory(fileLocation);
            fileName = fileName.Contains(".xml") ? fileName : fileName + ".xml";
            string to_write = SerializeObject<T>(data);
            StreamWriter writer;            
            FileInfo t = new FileInfo(fileLocation + "/" + fileName);
            if (t.Exists)
                t.Delete();

            writer = t.CreateText();
            writer.Write(to_write);
            writer.Close();
        }

        public static T LoadXML<T>(string fileLocation, string fileName)
        {
            fileName = fileName.Contains(".xml") ? fileName : fileName + ".xml";
            StreamReader r = File.OpenText(fileLocation + "/" + fileName);
            string _info = r.ReadToEnd();
            r.Close();
            
            return DeserializeObject<T>(_info);
        }        

        /// <summary>
        /// Loads a Text File
        /// </summary>
        /// <param name="filePath">Computer file path</param>
        /// <param name="fileName">File name without format</param>
        /// <param name="format">File format without dot (.)</param>
        /// <returns></returns>
        public static string[] LoadTextFile(string filePath, string fileName, string format)
        {
            List<string> lines = new List<string>();
            fileName = fileName.Contains("." + format) ? fileName : fileName + "." + format;
            StreamReader file = new StreamReader(filePath + "/" + fileName);
            string line = null;
            while((line = file.ReadLine()) != null)
            {
               lines.Add(line);
            }

            file.Close();
            return lines.ToArray();
        }

        private static T DeserializeObject<T>(string pXmlizedString)
        {
            XmlSerializer xs = new XmlSerializer(typeof(T));
            MemoryStream memoryStream = new MemoryStream(StringToUTF8ByteArray(pXmlizedString));
            //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            return (T)xs.Deserialize(memoryStream);
        }

        private static byte[] StringToUTF8ByteArray(string pXmlString)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        private static string SerializeObject<T>(object pObject)
        {
            string XmlizedString = null;
            MemoryStream memoryStream = new MemoryStream();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
            xmlTextWriter.WriteStartDocument(true);
            xmlTextWriter.Formatting = Formatting.Indented;
            xmlTextWriter.Indentation = 2;
            xs.Serialize(xmlTextWriter, pObject);
            memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
            XmlizedString = UTF8ByteArrayToString(memoryStream.ToArray());
            return XmlizedString;
        }

        private static string UTF8ByteArrayToString(byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        private static void CheckDirectory(string fileLocation)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(fileLocation);
            if (!dirInfo.Exists)
            {                
                dirInfo.Create();
            }
        }

    }

}

