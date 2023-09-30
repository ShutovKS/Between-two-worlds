#region

using Dialogue_Converter;
using static Dialogue_Converter.Constant.Path;

#endregion

const string language = @"\Rus";
string path = Directory.GetCurrentDirectory().Replace(
    @"Dialogue Converter\Dialogue Converter\bin\Debug\net7.0",
    string.Empty) + MainDirectory + language;

CharactersNames charactersNames = new CharactersNames(path + CharactersNamesPath);
CharactersAvatars charactersAvatars = new CharactersAvatars(path + CharactersAvatarsPath);

var dialogues = ParseDialogues(path + DialoguePath);
dialogues[0].ID = "Start";
SerializeTools.SerializeDialogues(dialogues, path + XmlPath);

return;

List<IPhrase> ParseDialogues(string dialoguePath)
{
    int id = 0;
    var dialogues = new List<IPhrase>();

    var dialoguesTexts = File.ReadAllLines(dialoguePath).Where(dialogue => !string.IsNullOrWhiteSpace(dialogue));

    foreach (string inputText in dialoguesTexts)
    {
        var phrases = ParseDialogue(id++, inputText);
        dialogues.AddRange(phrases);
    }

    string background = string.Empty;
    foreach (IPhrase dialogue in dialogues)
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
    int colonIndex = inputText.IndexOf(':');
    string character = inputText[..colonIndex].Trim();
    string text = inputText[(colonIndex + 1)..].TrimStart(':', ' ');

    IPhrase[] phrases = true switch
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
    string[] contents = content.Split('|');
    var responseList = new Responses.Response[contents.Length];

    for (int i = 0; i < contents.Length; i++)
    {
        string part = contents[i];
        string IDNext = string.Empty;
        if (part.Length > 0 && part[^1] == ']')
        {
            IDNext = ScrapingTools.GetId(id + 1);
            int openBracketIndex = part.IndexOf('[');
            int closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }
        if (ScrapingTools.TryGetUniqueIdInEnd(ref part, out string? uniqueId))
        {
            IDNext = uniqueId;
        }

        responseList[i] = new Responses.Response
        {
            AnswerText = part,
            IDNextDialog = IDNext
        };
    }

    Responses responses = new Responses
    {
        ID = ScrapingTools.GetId(id),
        ResponseList = responseList
    };

    return new IPhrase[] { responses };
}

IPhrase[] ParseSimpleDialogue(int id, string character, string part)
{
    string background = ScrapingTools.GetBackground(ref part);
    charactersAvatars.TryGetCharacterAvatarPath(part, out string? characterAvatarPath);
    charactersNames.TryGetCharacterName(part, out string? characterName);
    Phrase phrase = new Phrase
    {
        ID = ScrapingTools.GetId(id),
        IDNextDialog = ScrapingTools.GetId(id + 1),
        CharacterAvatarPath = characterAvatarPath,
        Name = characterName,
        Text = part,
        BackgroundPath = background,
        SoundEffect = null,
        ActionTrigger = ScrapingTools.GetActionTrigger(ref part)
    };

    if (ScrapingTools.TryGetUniqueIdInStart(ref part, out string? uniqueId))
    {
        phrase.ID = uniqueId!;
        phrase.Text = part;
    }

    return new IPhrase[] { phrase };
}

IPhrase[] ParseBranchingDialogues(int id, string character, string content)
{
    string background = ScrapingTools.GetBackground(ref content);
    string[] dialogueParts = content.Split('|');
    IPhrase[] dialogues = new IPhrase[dialogueParts.Length];

    for (int i = 0; i < dialogueParts.Length; i++)
    {
        string ID = ScrapingTools.GetId(id);
        string IDNext = ScrapingTools.GetId(id + 1);
        string part = dialogueParts[i];

        if (part[0] == '[')
        {
            int openBracketIndex = part.IndexOf('[');
            int closeBracketIndex = part.IndexOf(']');
            ID += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[(closeBracketIndex + 1)..];
        }

        if (part.Length > 0 && part[^1] == ']')
        {
            int openBracketIndex = part.IndexOf('[');
            int closeBracketIndex = part.IndexOf(']');
            IDNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }

        charactersAvatars.TryGetCharacterAvatarPath(part, out string? characterAvatarPath);
        charactersNames.TryGetCharacterName(part, out string? characterName);
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
