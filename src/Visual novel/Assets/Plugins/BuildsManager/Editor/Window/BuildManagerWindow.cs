using System.Collections.Generic;
using System.Linq;
using BuildsManager.Core;
using BuildsManager.Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BuildsManager.Window
{
    public class BuildManagerWindow : EditorWindow
    {
        private static GeneralBuildData Settings => MainManager.GeneralBuildData;

        private static string _settingsPath;

        private static bool _globalDataFoldout = true;
        private static bool _buildsDataFoldout = true;
        private static bool _showAddonsUsed = false;

        private static Vector2 _scrollPositionBuilds = Vector2.zero;

        [MenuItem("File/Builds Manager", false, 205)]
        public static void ShowWindow()
        {
            GetWindow<BuildManagerWindow>(false, "Builds Manager", true);

            MainManager.LoadSettings();
        }

        private void OnGUI()
        {
            if (Settings == null)
            {
                MainManager.LoadSettings();
            }

            DrawGlobalBuildData();

            DrawBuildDataList();

            DrawBuildsData();

            DrawBuildButtons();

            EditorUtility.SetDirty(Settings);
        }

        #region Main Drawers

        private static void DrawGlobalBuildData()
        {
            _globalDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_globalDataFoldout, "Global data");

            if (_globalDataFoldout)
            {
                ++EditorGUI.indentLevel;

                PlayerSettings.companyName = EditorGUILayout.TextField("Company name", PlayerSettings.companyName);
                PlayerSettings.productName = EditorGUILayout.TextField("Project name", PlayerSettings.productName);
                PlayerSettings.bundleVersion = EditorGUILayout.TextField("Version", PlayerSettings.bundleVersion);

                EditorGUILayout.Space(5);

                EditorGUILayout.LabelField("For Android");
                PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Android package version",
                    PlayerSettings.Android.bundleVersionCode);
                PlayerSettings.Android.keystorePass =
                    EditorGUILayout.TextField("Android keystore pass", PlayerSettings.Android.keystorePass);
                PlayerSettings.Android.keyaliasPass =
                    EditorGUILayout.TextField("Android keyalias pass", PlayerSettings.Android.keyaliasPass);

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        private static void DrawBuildsData()
        {
            _buildsDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_buildsDataFoldout, "Build data");

            if (_buildsDataFoldout)
            {
                ++EditorGUI.indentLevel;

                DrawConfiguredBuilds();

                DrawPathData();

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            return;

            static void DrawConfiguredBuilds()
            {
                EditorGUILayout.BeginVertical();

                Settings.generalScriptingDefineSymbols = EditorGUILayout.TextField("Scripting Define Symbols Default",
                    Settings.generalScriptingDefineSymbols);
                Settings.isReleaseBuild = EditorGUILayout.Toggle("Is Release build", Settings.isReleaseBuild);

                EditorGUILayout.EndVertical();
            }

            static void DrawPathData()
            {
                Settings.outputRoot = EditorGUILayout.TextField("Output root", Settings.outputRoot);
                Settings.middlePath = EditorGUILayout.TextField("Middle path", Settings.middlePath);
                Settings.dirPathForPostProcess =
                    EditorGUILayout.TextField("Dir path for process", Settings.dirPathForPostProcess);
            }
        }

        private static void DrawBuildDataList()
        {
            GUILayout.Label("Build Data List", EditorStyles.boldLabel);

            _scrollPositionBuilds = EditorGUILayout.BeginScrollView(_scrollPositionBuilds);
            EditorGUILayout.BeginHorizontal(GUILayout.Width(Settings.builds.Count * 250));

            if (Settings != null && Settings.builds != null)
            {
                for (var i = 0; i < Settings.builds.Count; i++)
                {
                    var settingsBuild = Settings.builds[i];

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField($"{settingsBuild.target}", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    settingsBuild.isEnabled = EditorGUILayout.Toggle("Enabled", settingsBuild.isEnabled);
                    settingsBuild.isCompress = EditorGUILayout.Toggle("Compress", settingsBuild.isCompress);
                    EditorGUILayout.EndHorizontal();

                    settingsBuild.target = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", settingsBuild.target);
                    settingsBuild.options =
                        (BuildOptions)EditorGUILayout.EnumFlagsField("Build Options", settingsBuild.options);
                    DrawAddonsUsed(settingsBuild);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Remove", GUILayout.Width(80)))
                    {
                        Settings.builds.RemoveAt(i);
                        break;
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Build", GUILayout.Width(80)))
                    {
                        MainManager.RunBuild(settingsBuild);
                        break;
                    }

                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Build"))
            {
                Settings.builds?.Add(new BuildData());
            }

            return;

            static void DrawAddonsUsed(BuildData buildData)
            {
                if (Settings == null || Settings.addonsUsedData == null || Settings.addonsUsedData.AddonsUsed == null)
                {
                    return;
                }

                _showAddonsUsed = EditorGUILayout.BeginFoldoutHeaderGroup(_showAddonsUsed, "Addons Used");

                if (_showAddonsUsed)
                {
                    ++EditorGUI.indentLevel;

                    var allAddonsUsed = Settings.addonsUsedData.AddonsUsed;
                    List<AddonUsedInformation> selectedAddons;

                    if (buildData.addonsUsed == null)
                    {
                        selectedAddons = allAddonsUsed
                            .Select(addonUsedDetailed => new AddonUsedInformation(addonUsedDetailed.Name))
                            .ToList();
                    }
                    else
                    {
                        selectedAddons = buildData.addonsUsed;
                    }


                    foreach (var addonUsed in allAddonsUsed.Where(addonUsed =>
                                 selectedAddons.All(x => x.Name != addonUsed.Name)))
                    {
                        selectedAddons.Add(new AddonUsedInformation(addonUsed.Name));
                    }

                    selectedAddons.RemoveAll(x => allAddonsUsed.All(y => y.Name != x.Name));

                    for (var index = 0; index < selectedAddons.Count; index++)
                    {
                        var addonUsed = selectedAddons[index];
                        
                        selectedAddons[index] = new AddonUsedInformation()
                        {
                            Name = addonUsed.Name,
                            IsUsed = EditorGUILayout.ToggleLeft(addonUsed.Name, addonUsed.IsUsed)
                        };
                    }

                    buildData.addonsUsed = selectedAddons;

                    --EditorGUI.indentLevel;
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }

        private static void DrawBuildButtons()
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.773f, 0.345098f, 0.345098f);

            EditorGUILayout.Space(2.5f);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button($"Open AddonsUsed Data", GUILayout.Height(20f)))
            {
                MainManager.OpenAddonsUsedData();
            }

            if (GUILayout.Button($"Build All", GUILayout.Height(20f)))
            {
                MainManager.RunBuild();
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = prevColor;

            EditorGUILayout.Space(5f);
        }

        #endregion

        #region Callbacks

        private static void OnBuildSelectionChanged(ReorderableList list)
        {
        }

        private static void OnBuildAdd(object target)
        {
            Settings.builds.Add((target as BuildData)?.Clone() as BuildData);
        }

        #endregion
    }
}