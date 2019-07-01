using System.IO;
using System.Xml.Serialization;

public static class HelperScript {
    
    //Serialize
    public static string Serialize<T> (this T toSerialize)
    {
        XmlSerializer l_xml = new XmlSerializer(typeof(T));
        StringWriter l_writer = new StringWriter();

        l_xml.Serialize(l_writer, toSerialize);
        return l_writer.ToString();
    }

    //Deserialize
    public static T Deserialize <T> (this string toDeserialize)
    {
        XmlSerializer l_xml = new XmlSerializer(typeof(T));
        StringReader l_reader = new StringReader(toDeserialize);
        return (T)l_xml.Deserialize(l_reader);
    }
}
