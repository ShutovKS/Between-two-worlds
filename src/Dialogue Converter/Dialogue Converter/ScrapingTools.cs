﻿namespace Dialogue_Converter;

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

    public static string? GetBackground(ref string content)
    {
        if (!content.Contains('<')) return null;

        var openBracketIndex = content.IndexOf('<');
        var closeBracketIndex = content.IndexOf('>');
        var background = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = content[..openBracketIndex] + content[(closeBracketIndex + 1)..];
        return background;
    }

    public static string GetActionTrigger(ref string content)
    {
        if (!content.Contains('{')) return string.Empty;

        var openBracketIndex = content.IndexOf('{');
        var closeBracketIndex = content.IndexOf('}');
        var actionTrigger = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = content[..openBracketIndex] + content[(closeBracketIndex + 1)..];
        return actionTrigger;
    }

    public static string GetId(int id)
    {
        return $"id{id}.";
    }

    private static string GetUniqueId(ref string content)
    {
        var openBracketIndex = content.IndexOf('@');
        string contentTemp = content[..openBracketIndex] + '.' + content[(openBracketIndex + 1)..];
        var closeBracketIndex = contentTemp.IndexOf('@');
        var uniqueId = content.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
        content = contentTemp[..openBracketIndex] + contentTemp[(closeBracketIndex + 1)..];
        return uniqueId;
    }
}
