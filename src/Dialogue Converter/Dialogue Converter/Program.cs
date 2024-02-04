#region

using Dialogue_Converter;
using static Dialogue_Converter.Constant.Path;

#endregion

var path = Directory.GetCurrentDirectory().Replace(
    @"Dialogue Converter\Dialogue Converter\bin\Debug\net7.0",
    string.Empty);

var directories = Directory.GetDirectories(path + MainDirectory)
    .Select(directory => directory.Replace(path + MainDirectory, string.Empty));

foreach (var directory in directories)
{
    var mainPath = path + MainDirectory + directory;
    var localizationPath = path + LocalizationDirectory + directory;

    var charactersNames = new CharactersNames(mainPath + CharactersNamesPath);
    var charactersAvatars = new CharactersAvatars(mainPath + CharactersAvatarsPath);
    var dialogues = ParseDialogues(mainPath + DialoguePath, charactersAvatars, charactersNames);
    dialogues[0].ID = "Start";

    if (!Directory.Exists(localizationPath)) Directory.CreateDirectory(localizationPath);
    SerializeTools.SerializeDialogues(dialogues, localizationPath + XmlPath);
    File.Copy(mainPath + LastWordsPath, localizationPath + LastWordsPath, true);
    File.Copy(mainPath + UILocalisationPath, localizationPath + UILocalisationPath, true);
    File.Copy(mainPath + MainPath, localizationPath + MainPath, true);
    File.Copy(mainPath + FlagPath, localizationPath + FlagPath, true);
}

return;

List<IPhrase> ParseDialogues(string dialoguePath, CharactersAvatars charactersAvatars, CharactersNames charactersNames)
{
    var id = 0;
    var dialogues = new List<IPhrase>();

    var dialoguesTexts = File.ReadAllLines(dialoguePath).Where(dialogue => !string.IsNullOrWhiteSpace(dialogue));

    foreach (var inputText in dialoguesTexts)
    {
        var phrases = ParseDialogue(id++, inputText, charactersAvatars, charactersNames);
        dialogues.AddRange(phrases);
    }

    var background = string.Empty;
    var soundEffect = string.Empty;
    foreach (var dialogue in dialogues)
    {
        if (dialogue is not Phrase phrase) continue;

        if (phrase.BackgroundPath == null) phrase.BackgroundPath = background;
        else background = phrase.BackgroundPath;

        if (phrase.SoundEffect == null) phrase.SoundEffect = soundEffect;
        else soundEffect = phrase.SoundEffect;
    }

    return dialogues;
}

IEnumerable<IPhrase> ParseDialogue(int id, string inputText, CharactersAvatars charactersAvatars,
    CharactersNames charactersNames)
{
    var colonIndex = inputText.IndexOf(':');
    var character = inputText[..colonIndex].Trim();
    var text = inputText[(colonIndex + 1)..].TrimStart(':', ' ');

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

    for (var i = 0; i < contents.Length; i++)
    {
        var part = contents[i];
        var idNext = string.Empty;
        if (part.Length > 0 && part[^1] == ']')
        {
            idNext = ScrapingTools.GetId(id + 1);
            var openBracketIndex = part.IndexOf('[');
            var closeBracketIndex = part.IndexOf(']');
            idNext += part.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
            part = part[..openBracketIndex] + part[(closeBracketIndex + 1)..];
        }

        if (ScrapingTools.TryGetUniqueIdInEnd(ref part, out var uniqueId))
        {
            idNext = uniqueId!;
        }

        responseList[i] = new Responses.Response
        {
            AnswerText = part,
            IDNextDialog = idNext
        };
    }

    var responses = new Responses
    {
        ID = ScrapingTools.GetId(id),
        ResponseList = responseList
    };

    return new IPhrase[] { responses };
}

IPhrase[] ParseSimpleDialogue(int id, string character, string part, CharactersAvatars charactersAvatars,
    CharactersNames charactersNames)
{
    charactersAvatars.TryGetCharacterAvatarPath(character, out var characterAvatarPath);
    charactersNames.TryGetCharacterName(character, out var characterName);
    ScrapingTools.TryGetBackground(ref part, out var backgroundPath);
    ScrapingTools.TryGetSoundEffect(ref part, out var soundEffect);
    ScrapingTools.TryGetActionTrigger(ref part, out var actionTrigger);
    ScrapingTools.TryGetUniqueIdInStart(ref part, out var uniqueId);

    var phrase = new Phrase
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

IPhrase[] ParseBranchingDialogues(int id, string character, string content, CharactersAvatars charactersAvatars,
    CharactersNames charactersNames)
{
    ScrapingTools.TryGetBackground(ref content, out var background);
    ScrapingTools.TryGetSoundEffect(ref content, out var soundEffect);
    charactersAvatars.TryGetCharacterAvatarPath(character, out var characterAvatarPath);
    charactersNames.TryGetCharacterName(character, out var characterName);

    var dialogueParts = content.Split('|');
    var dialogues = new IPhrase[dialogueParts.Length];

    for (var i = 0; i < dialogueParts.Length; i++)
    {
        var ID = ScrapingTools.GetId(id);
        var IDNext = ScrapingTools.GetId(id + 1);
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