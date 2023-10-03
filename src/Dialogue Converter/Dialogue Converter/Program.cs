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

    var background = string.Empty;
    var soundEffect = string.Empty;
    foreach (IPhrase dialogue in dialogues)
    {
        if (dialogue is not Phrase phrase) continue;

        if (phrase.BackgroundPath == null) phrase.BackgroundPath = background;
        else background = phrase.BackgroundPath;

        if (phrase.SoundEffect == null) phrase.SoundEffect = soundEffect;
        else soundEffect = phrase.SoundEffect;
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
        string idNext = string.Empty;
        if (part.Length > 0 && part[^1] == ']')
        {
            idNext = ScrapingTools.GetId(id + 1);
            int openBracketIndex = part.IndexOf('[');
            int closeBracketIndex = part.IndexOf(']');
            idNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }
        if (ScrapingTools.TryGetUniqueIdInEnd(ref part, out string? uniqueId))
        {
            idNext = uniqueId!;
        }

        responseList[i] = new Responses.Response
        {
            AnswerText = part,
            IDNextDialog = idNext
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
    ScrapingTools.TryGetBackground(ref part, out string? backgroundPath);
    ScrapingTools.TryGetSoundEffect(ref part, out string? soundEffect);
    ScrapingTools.TryGetActionTrigger(ref part, out string? actionTrigger);
    ScrapingTools.TryGetUniqueIdInStart(ref part, out string? uniqueId);

    Phrase phrase = new Phrase
    {
        ID = uniqueId ?? ScrapingTools.GetId(id),
        IDNextDialog = ScrapingTools.GetId(id + 1),
        ActionTrigger = actionTrigger,
        CharacterAvatarPath = characterAvatarPath,
        Name = characterName,
        Text = part,
        BackgroundPath = backgroundPath,
        SoundEffect = soundEffect
    };

    return new IPhrase[] { phrase };
}

IPhrase[] ParseBranchingDialogues(int id, string character, string content, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
{
    ScrapingTools.TryGetBackground(ref content, out string? background);
    ScrapingTools.TryGetSoundEffect(ref content, out string? soundEffect);
    var ID = ScrapingTools.GetId(id);
    var IDNext = ScrapingTools.GetId(id + 1);
    charactersAvatars.TryGetCharacterAvatarPath(character, out string? characterAvatarPath);
    charactersNames.TryGetCharacterName(character, out string? characterName);

    var dialogueParts = content.Split('|');
    var dialogues = new IPhrase[dialogueParts.Length];

    for (int i = 0; i < dialogueParts.Length; i++)
    {
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

        dialogues[i] = new Phrase
        {
            ID = ID,
            IDNextDialog = IDNext,
            CharacterAvatarPath = characterAvatarPath,
            Name = characterName,
            Text = part,
            BackgroundPath = background,
            SoundEffect = soundEffect
        };
    }

    return dialogues;
}
