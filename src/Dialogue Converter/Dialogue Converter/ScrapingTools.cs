namespace Dialogue_Converter;

public static class ScrapingTools
{
    public static bool TryGetUniqueIdInStart(ref string content, out string? uniqueId)
    {
        if (content.Length <= 0 || content[0] != '@')
        {
            uniqueId = null;
            return false;
        }

        uniqueId = GetUniqueId(ref content);
        return true;
    }

    public static bool TryGetUniqueIdInEnd(ref string content, out string? uniqueId)
    {
        if (content.Length <= 0 || content[^1] != '@')
        {
            uniqueId = null;
            return false;
        }

        uniqueId = GetUniqueId(ref content);
        return true;
    }

    public static bool TryGetBackground(ref string content, out string? background)
    {
        if (!content.Contains('<'))
        {
            background = null;
            return false;
        }

        var openBracketIndex = content.IndexOf('<');
        var closeBracketIndex = content.IndexOf('>');
        background = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = content[..openBracketIndex] + content[(closeBracketIndex + 1)..];
        return true;
    }

    public static bool TryGetActionTrigger(ref string content, out string? actionTrigger)
    {
        if (!content.Contains('{'))
        {
            actionTrigger = null;
            return false;
        }

        var openBracketIndex = content.IndexOf('{');
        var closeBracketIndex = content.IndexOf('}');
        actionTrigger = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = content[..openBracketIndex] + content[(closeBracketIndex + 1)..];
        return true;
    }

    public static bool TryGetSoundEffect(ref string content, out string? soundEffect)
    {
        if (!content.Contains('$'))
        {
            soundEffect = null;
            return false;
        }

        var openBracketIndex = content.IndexOf('$');
        var contentTemp = content[..openBracketIndex] + '.' + content[(openBracketIndex + 1)..];
        var closeBracketIndex = contentTemp.IndexOf('$');
        soundEffect = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = contentTemp[..openBracketIndex] + contentTemp[(closeBracketIndex + 1)..];
        return true;
    }


    public static string GetId(int id)
    {
        return $"id{id}.";
    }

    private static string GetUniqueId(ref string content)
    {
        var openBracketIndex = content.IndexOf('@');
        var contentTemp = content[..openBracketIndex] + '.' + content[(openBracketIndex + 1)..];
        var closeBracketIndex = contentTemp.IndexOf('@');
        var uniqueId = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = contentTemp[..openBracketIndex] + contentTemp[(closeBracketIndex + 1)..];
        return uniqueId;
    }
}
