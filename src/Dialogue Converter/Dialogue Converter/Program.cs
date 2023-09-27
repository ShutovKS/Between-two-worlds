#region

using System.Xml.Serialization;
using Dialogue_Converter;

#endregion

var path = Directory.GetCurrentDirectory().Replace(
    @"Dialogue Converter\Dialogue Converter\bin\Debug\net7.0",
    string.Empty);

var xmlPath = path + @"Scenario\Rus\Dialogues Test.xml";
var dialoguePath = path + @"Scenario\Rus\Dialogue script.txt";

var charactersNames = new CharactersNames(path + @"Scenario\Rus");
var charactersAvatars = new CharactersAvatars(path + @"Scenario\Rus");

var dialogues = ParseDialogues(dialoguePath);
dialogues[0].ID = "Start";
SerializeDialogues(dialogues, xmlPath);

return;

List<IPhrase> ParseDialogues(string dialoguePath)
{
    var id = 0;
    var dialogues = new List<IPhrase>();

    var dialoguesTexts = File.ReadAllLines(dialoguePath).Where(dialogue => !string.IsNullOrWhiteSpace(dialogue));

    foreach (var inputText in dialoguesTexts)
    {
        var phrases = ParseDialogue(id++, inputText);
        dialogues.AddRange(phrases);
    }

    var background = string.Empty;
    foreach (var dialogue in dialogues)
    {
        if (dialogue is not Phrase phrase) continue;
        if (phrase.BackgroundPath == null)
        {
            phrase.BackgroundPath = background;
        }
        else
        {
            background = phrase.BackgroundPath;
        }
    }

    return dialogues;
}

IEnumerable<IPhrase> ParseDialogue(int id, string inputText)
{
    var colonIndex = inputText.IndexOf(':');
    var character = inputText[..colonIndex].Trim();
    var text = inputText[(colonIndex + 1)..].TrimStart(':', ' ');

    var phrases = true switch
    {
        _ when text.Contains('#') => ParseResponseDialogue(id, text),
        _ when text.Contains('|') => ParseBranchingDialogues(id, character, text),
        _ => ParseSimpleDialogue(id, character, text)
    };

    return phrases;
}

IPhrase[] ParseResponseDialogue(int id, string content)
{
    content = content.Replace("#", string.Empty);
    var contents = content.Split('|');
    var responseList = new Responses.Response[contents.Length];

    for (var i = 0; i < contents.Length; i++)
    {
        var part = contents[i];
        var IDNext = string.Empty;
        if (part.Length > 0 && part[^1] == ']')
        {
            IDNext = GetId(id + 1);
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }
        if (ScrapingTools.TryGetUniqueIdInEnd(ref part, out var uniqueId))
        {
            IDNext = uniqueId;
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

IPhrase[] ParseSimpleDialogue(int id, string character, string part)
{
    var background = ScrapingTools.GetBackground(ref part);
    charactersAvatars.TryGetCharacterAvatarPath(part, out var characterAvatarPath);
    charactersNames.TryGetCharacterName(part, out var characterName);
    var phrase = new Phrase
    {
        ID = GetId(id),
        IDNextDialog = GetId(id + 1),
        CharacterAvatarPath = characterAvatarPath,
        Name = characterName,
        Text = part,
        BackgroundPath = background,
        SoundEffect = null
    };

    if (ScrapingTools.TryGetUniqueIdInStart(ref part, out var uniqueId))
    {
        phrase.ID = uniqueId!;
        phrase.Text = part;
    }

    return new IPhrase[] { phrase };
}

IPhrase[] ParseBranchingDialogues(int id, string character, string content)
{
    var background = ScrapingTools.GetBackground(ref content);
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
            part = part[(closeBracketIndex + 1)..];
        }

        if (part.Length > 0 && part[^1] == ']')
        {
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }

        charactersAvatars.TryGetCharacterAvatarPath(part, out var characterAvatarPath);
        charactersNames.TryGetCharacterName(part, out var characterName);
        dialogues[i] = new Phrase
        {
            ID = ID,
            IDNextDialog = IDNext,
            CharacterAvatarPath = characterAvatarPath,
            Name = characterName,
            Text = part,
            BackgroundPath = background,
            SoundEffect = null
        };
    }

    return dialogues;
}

void SerializeDialogues(List<IPhrase> dialogues, string xmlPath)
{
    var serializer = new XmlSerializer(typeof(object[]), new[] { typeof(Phrase), typeof(Responses) });
    using var stream = File.Create(xmlPath);
    serializer.Serialize(stream, dialogues.ToArray());
}

string GetId(int id)
{
    return $"id{id}.";
}
