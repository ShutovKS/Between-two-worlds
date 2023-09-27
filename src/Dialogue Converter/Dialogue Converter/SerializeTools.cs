using System.Xml.Serialization;

namespace Dialogue_Converter;

public static class SerializeTools
{
    static public void SerializeDialogues(List<IPhrase> dialogues, string xmlPath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(object[]), new[] { typeof(Phrase), typeof(Responses) });
        using FileStream stream = File.Create(xmlPath);
        serializer.Serialize(stream, dialogues.ToArray());
    }
}
