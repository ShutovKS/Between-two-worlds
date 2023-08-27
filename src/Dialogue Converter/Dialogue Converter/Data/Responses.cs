namespace Dialogue_Converter;

[Serializable]
public class Responses : IPhrase
{
    public string ID { get; set; }
    public Response[] ResponseList { get; set; }

    [Serializable]
    public class Response
    {
        public string AnswerText { get; set; }
        public string IDNextDialog { get; set; }
    }
}