using System;
using Editor.ScriptingDefineSymbols;
using UnityEditor;

namespace Editor.BuildManager.Core
{
    public static class ConvertToStrings
    {
        public static string GetPathWithVars(DateTime usedDate, BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{usedDate.Date.Year % 100}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{usedDate.Date.Year}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{usedDate.Hour}_{usedDate.Minute}");
            s = s.Replace("$ADDONS", $"{AddonsUsed.GetAddonsUsedName(data.addonsUsed)}");
            s = s.Replace("$EXECUTABLE", GetBuildTargetExecutable(data.target));
            return s;
        }

        public static string GetPathWithVarsForZip(DateTime usedDate, BuildData data, string s)
        {
            s = s.Replace("$NAME", GetProductName());
            s = s.Replace("$PLATFORM", ConvertBuildTargetToString(data.target));
            s = s.Replace("$VERSION", PlayerSettings.bundleVersion);
            s = s.Replace("$DATESHORT", $"{usedDate.Date.Year % 100}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEARSHORT", $"{usedDate.Date.Year % 100}");
            s = s.Replace("$DATE", $"{usedDate.Date.Year}_{usedDate.Date.Month}_{usedDate.Date.Day}");
            s = s.Replace("$YEAR", $"{usedDate.Date.Year}");
            s = s.Replace("$MONTH", $"{usedDate.Date.Month}");
            s = s.Replace("$DAY", $"{usedDate.Date.Day}");
            s = s.Replace("$TIME", $"{usedDate.Hour}_{usedDate.Minute}");

            s = s.Contains("$EXECUTABLE")
                ? s.Replace("$EXECUTABLE", GetBuildTargetExecutable(data.target))
                : s + ".zip";

            return s;
        }

        public static string ConvertBuildTargetToString(BuildTarget target)
        {
            return target switch
            {
                BuildTarget.StandaloneOSX => "OSX",
                BuildTarget.StandaloneWindows => "Windows32",
                BuildTarget.StandaloneWindows64 => "Windows64",
                BuildTarget.StandaloneLinux64 => "Linux",
                _ => target.ToString()
            };
        }

        public static string GetProductName()
        {
            return PlayerSettings.productName
                    .Replace(' ', '_')
                    .Replace('/', '_')
                    .Replace('\\', '_')
                    .Replace(':', '_')
                    .Replace('*', '_')
                    .Replace('?', '_')
                    .Replace('"', '_')
                    .Replace('<', '_')
                    .Replace('>', '_')
                    .Replace('|', '_')
                ;
        }

        public static string GetBuildTargetExecutable(BuildTarget target)
        {
            return target switch
            {
                BuildTarget.StandaloneWindows => ".exe",
                BuildTarget.StandaloneWindows64 => ".exe",
                BuildTarget.StandaloneLinux64 => "x86_64",
                BuildTarget.StandaloneOSX => "",
                BuildTarget.iOS => ".ipa",
                BuildTarget.Android => ".apk",
                BuildTarget.WebGL => "",
                _ => ""
            };
        }
    }
}