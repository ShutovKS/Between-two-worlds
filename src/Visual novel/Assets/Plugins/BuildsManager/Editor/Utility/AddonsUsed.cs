using BuildsManager.Data;

namespace BuildsManager.Utility
{
    public static class AddonsUsed
    {
        public static string GetScriptingDefineSymbols(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.None => string.Empty,
                AddonsUsedType.YandexGameSDK => "YG_PLUGIN_YANDEX_GAME" + ";" + "YG_SERVICES",
                _ => string.Empty
            };
        }

        public static string GetAddonName(AddonsUsedType type)
        {
            return type switch
            {
                AddonsUsedType.None => string.Empty,
                AddonsUsedType.YandexGameSDK => "YandexGame",
                _ => string.Empty
            };
        }
    }
}