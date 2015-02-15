using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace OnlineMessenger.Mailer
{
    public static class SerializationTools
    {
        public static string ToXml(this object target)
        {
            var serializer = new XmlSerializer(target.GetType());
            using (var memoryStream = new MemoryStream())
            {
                serializer.Serialize(memoryStream, target);
                return Encoding.ASCII.GetString(memoryStream.ToArray());
            }
        }
        public static T FromXml<T>(string xmlString)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(xmlString);
                    writer.Flush();
                    stream.Position = new int();
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stream);
                }
            }
        }
    }
}
