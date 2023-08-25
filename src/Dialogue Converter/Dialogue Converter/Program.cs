#region

using System.Xml.Serialization;
using Dialogue_Converter;

#endregion

var path = Directory.GetCurrentDirectory().Replace(
    @"Dialogue Converter\Dialogue Converter\bin\Debug\net7.0",
    string.Empty);

var xmlPath = path + @"Scenario\Rus Demo\Dialogues.xml";
var dialoguePath = path + @"Scenario\Rus Demo\Dialogue script.txt";

var dialogues = ParseDialogues(dialoguePath);
dialogues[0].ID = "Start";
SerializeDialogues(dialogues, xmlPath);

return;

static List<IPhrase> ParseDialogues(string dialoguePath)
{
    var id = 0;
    var dialogues = new List<IPhrase>();
    var list = new List<string>();
    foreach (var dialogue in File.ReadAllLines(dialoguePath))
    {
        if (!string.IsNullOrWhiteSpace(dialogue))
            list.Add(dialogue);
    }

    var dialoguesTexts = list.ToArray();

    foreach (var inputText in dialoguesTexts)
    {
        var phrases = ParseDialogue(ref id, inputText);
        foreach (var phrase in phrases)
        {
            dialogues.Add(phrase);
        }
    }

    return dialogues;
}

static IPhrase[] ParseDialogue(ref int id, string inputText)
{
    var colonIndex = inputText.IndexOf(':');
    var firstWord = inputText[..colonIndex].Trim();
    var remainingText = inputText[(colonIndex + 1)..].TrimStart(':', ' ');

    var phrases = true switch
    {
        _ when remainingText.Contains('#') => ParseResponseDialogue(ref id, firstWord, remainingText),
        _ when remainingText.Contains('|') => ParseBranchingDialogues(ref id, firstWord, remainingText),
        _ => ParseSimpleDialogue(ref id, firstWord, remainingText)
    };

    id++;
    return phrases;
}

static IPhrase[] ParseResponseDialogue(ref int id, string character, string content)
{
    content = content.Replace("#", string.Empty);
    var contents = content.Split('|');
    var responseList = new Responses.Response[contents.Length];

    for (var i = 0; i < contents.Length; i++)
    {
        var part = contents[i];
        var IDNext = GetId(id + 1);

        if (part[^1] == ']')
        {
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex];
        }

        responseList[i] = new Responses.Response
        {
            AnswerText = part,
            IDNextDialog = IDNext
        };
    }

    var responses = new Responses
    {
        ID = GetId(id),
        ResponseList = responseList
    };

    return new IPhrase[] { responses };
}

static IPhrase[] ParseSimpleDialogue(ref int id, string character, string content)
{
    var phrase = new Phrase
    {
        ID = GetId(id),
        IDNextDialog = GetId(id + 1),
        CharacterAvatarPath = GetCharacterImage(character) ?? "N U L L",
        Name = GetCharacterName(character) ?? "N U L L",
        Text = content,
        BackgroundPath = "null",
        SoundEffect = "null"
    };

    return new IPhrase[] { phrase };
}

static IPhrase[] ParseBranchingDialogues(ref int id, string character, string content)
{
    var dialogueParts = content.Split('|');
    var dialogues = new IPhrase[dialogueParts.Length];

    for (var i = 0; i < dialogueParts.Length; i++)
    {
        var ID = GetId(id);
        var IDNext = GetId(id + 1);
        var part = dialogueParts[i];

        if (part[0] == '[')
        {
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            ID += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex];
        }

        if (part.Length > 0 && part[^1] == ']')
        {
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex];
        }

        dialogues[i] = new Phrase
        {
            ID = ID,
            IDNextDialog = IDNext,
            CharacterAvatarPath = GetCharacterImage(character) ?? "N U L L",
            Name = GetCharacterName(character) ?? "N U L L",
            Text = part,
            BackgroundPath = "null",
            SoundEffect = "null"
        };
    }

    return dialogues;
}

static void SerializeDialogues(List<IPhrase> dialogues, string xmlPath)
{
    var serializer = new XmlSerializer(typeof(object[]), new[] { typeof(Phrase), typeof(Responses) });
    using var stream = File.Create(xmlPath);
    serializer.Serialize(stream, dialogues.ToArray());
}

static string? GetCharacterName(string character)
{
    return character switch
    {
        "CharacterH" => "Герой",
        "CharacterD" => "Доктор",
        "CharacterG" => "Девушка",
        "CharacterGF" => "Подруга",
        "CharacterW" => "Офицант",

        "OtherDoor" => "Дверь",
        "OtherSignatureUnderPainting" => "Подпись под картиной",
        "OtherPhone" => "Телефон",

        _ => throw new Exception($"No character name on character: {character}")
    };
}

static string? GetCharacterImage(string character)
{
    return character switch
    {
        "CharacterH" => "CharacterHero",
        "CharacterD" => "CharacterDoctor",
        "CharacterG" => "CharacterGirl",
        "CharacterGF" => "CharacterGirlfriend",
        "CharacterW" => "CharacterWaiter",


        _ => throw new Exception($"No character image on character: {character}")
    };
}

static string GetId(int id)
{
    return $"id{id}.";
}