#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Infrastructure.Services.LocalisationDataLoad
{
    public class LocalisationDataLoadServiceEditor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get; }

        public void OnPreprocessBuild(BuildReport report)
        {
            ShowLocalizations(false);
        }

        [MenuItem("Tools/Localization/Show localizations")]
        private static void ShowLocalizations()
        {
            ShowLocalizations(true);
        }

        private static void ShowLocalizations(bool isShow)
        {
            var path = Path.Combine(Application.dataPath, @"Project Files\Resources\Localizations");
            
            var directoriesNames = Directory.GetDirectories(path);

            for (var index = 0; index < directoriesNames.Length; index++)
            {
                var directoryName = directoriesNames[index];
                if (!File.Exists(Path.Combine(directoryName, "Main.txt")))
                {
                    continue;
                }

                var name = Path.GetFileName(directoryName);
                directoriesNames[index] = name;
            }

            var localizations = string.Join("\n", directoriesNames);

            if (!File.Exists(Path.Combine(path, "Localizations" + ".txt")))
            {
                File.Create(Path.Combine(path, "Localizations" + ".txt")).Close();
            }

            File.WriteAllText(Path.Combine(path, "Localizations" + ".txt"), localizations);

            AssetDatabase.Refresh();

            if (isShow)
            {
                EditorUtility.DisplayDialog("Localizations", localizations, "Ok");
            }
        }
    }
}
#endif