#region

using Dialogue_Converter;
using static Dialogue_Converter.Constant.Path;

#endregion

string path = Directory.GetCurrentDirectory().Replace(
    @"Dialogue Converter\Dialogue Converter\bin\Debug\net7.0",
    string.Empty);

var directories = Directory.GetDirectories(path + MainDirectory).Select(directory => directory.Replace(path + MainDirectory, string.Empty));

foreach (string directory in directories)
{
    var mainPath = path + MainDirectory + directory;
    var localizationPath = path + LocalizationDirectory + directory;

    CharactersNames charactersNames = new CharactersNames(mainPath + CharactersNamesPath);
    CharactersAvatars charactersAvatars = new CharactersAvatars(mainPath + CharactersAvatarsPath);
    var dialogues = ParseDialogues(mainPath + DialoguePath, charactersAvatars, charactersNames);
    dialogues[0].ID = "Start";

    if (!Directory.Exists(localizationPath)) Directory.CreateDirectory(localizationPath);
    SerializeTools.SerializeDialogues(dialogues, localizationPath + XmlPath);
    File.Copy(mainPath + LastWordsPath, localizationPath + LastWordsPath, true);
    File.Copy(mainPath + UILocalisationPath, localizationPath + UILocalisationPath, true);
    File.Copy(mainPath + MainPath, localizationPath + MainPath, true);
}
return;

List<IPhrase> ParseDialogues(string dialoguePath, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
{
    int id = 0;
    var dialogues = new List<IPhrase>();

    var dialoguesTexts = File.ReadAllLines(dialoguePath).Where(dialogue => !string.IsNullOrWhiteSpace(dialogue));

    foreach (string inputText in dialoguesTexts)
    {
        var phrases = ParseDialogue(id++, inputText, charactersAvatars, charactersNames);
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

IEnumerable<IPhrase> ParseDialogue(int id, string inputText, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
{
    int colonIndex = inputText.IndexOf(':');
    string character = inputText[..colonIndex].Trim();
    string text = inputText[(colonIndex + 1)..].TrimStart(':', ' ');

    var phrases = true switch
    {
        _ when text.Contains('#') => ParseResponseDialogue(id, text),
        _ when text.Contains('|') => ParseBranchingDialogues(id, character, text, charactersAvatars, charactersNames),
        _ => ParseSimpleDialogue(id, character, text, charactersAvatars, charactersNames)
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

IPhrase[] ParseSimpleDialogue(int id, string character, string part, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
{
    charactersAvatars.TryGetCharacterAvatarPath(character, out string? characterAvatarPath);
    charactersNames.TryGetCharacterName(character, out string? characterName);
    var background = ScrapingTools.GetBackground(ref part);
    var actionTrigger = ScrapingTools.GetActionTrigger(ref part);
    Phrase phrase = new Phrase
    {
        ID = ScrapingTools.GetId(id),
        IDNextDialog = ScrapingTools.GetId(id + 1),
        ActionTrigger = actionTrigger,
        CharacterAvatarPath = characterAvatarPath,
        Name = characterName,
        Text = part,
        BackgroundPath = background,
        SoundEffect = null,
    };

    if (ScrapingTools.TryGetUniqueIdInStart(ref part, out string? uniqueId))
    {
        phrase.ID = uniqueId!;
        phrase.Text = part;
    }

    return new IPhrase[] { phrase };
}

IPhrase[] ParseBranchingDialogues(int id, string character, string content, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
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

        charactersAvatars.TryGetCharacterAvatarPath(character, out string? characterAvatarPath);
        charactersNames.TryGetCharacterName(character, out string? characterName);
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
