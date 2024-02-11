using BuildsManager.Data;

namespace BuildsManager.Utility
{
    public static class AddonsUsed
    {
        public static string GetScriptingDefineSymbols(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.YandexGameSDK => "YG_SERVICES",
                _ => string.Empty
            };
        }

        public static string GetAddonName(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.YandexGameSDK => "YandexGame",
                _ => string.Empty
            };
        }
    }
}