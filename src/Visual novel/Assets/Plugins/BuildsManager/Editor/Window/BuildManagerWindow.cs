using BuildsManager.Core;
using BuildsManager.Data;
using BuildsManager.Window.DrawTools;
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

        private static Vector2 _scrollPosSequence = Vector2.zero;

        private static ReorderableList _buildList;

        [MenuItem("File/Builds Manager", false, 205)]
        public static void ShowWindow()
        {
            _buildList = null;

            GetWindow(typeof(BuildManagerWindow), false, "Builds Manager", true);

            MainManager.LoadSettings();
        }

        private void OnGUI()
        {
            if (Settings == null)
            {
                MainManager.LoadSettings();
            }

            _globalDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_globalDataFoldout, "Глобальные данные");

            if (_globalDataFoldout)
            {
                ++EditorGUI.indentLevel;

                DrawGlobalBuildData();

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            DrawSelectedSequenceData();

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            _buildsDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_buildsDataFoldout, "Данные билда");

            if (_buildsDataFoldout)
            {
                ++EditorGUI.indentLevel;

                DrawConfiguredBuilds();

                DrawPathData();

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            DrawBuildButtons();

            EditorGUILayout.Space(5f);

            EditorUtility.SetDirty(Settings);
        }

        #region Main Drawers

        private void DrawGlobalBuildData()
        {
            PlayerSettings.companyName = EditorGUILayout.TextField("Название компании", PlayerSettings.companyName);
            PlayerSettings.productName = EditorGUILayout.TextField("Название проекта", PlayerSettings.productName);
            PlayerSettings.bundleVersion = EditorGUILayout.TextField("Версия", PlayerSettings.bundleVersion);

            EditorGUILayout.Space(5);

            EditorGUILayout.LabelField("Для Android");
            PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Версия пакета Android",
                PlayerSettings.Android.bundleVersionCode);
            PlayerSettings.Android.keystorePass =
                EditorGUILayout.TextField("Android keystore pass", PlayerSettings.Android.keystorePass);
            PlayerSettings.Android.keyaliasPass =
                EditorGUILayout.TextField("Android keyalias pass", PlayerSettings.Android.keyaliasPass);
        }

        private static void DrawBuildButtons()
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.773f, 0.345098f, 0.345098f);

            EditorGUILayout.Space(2.5f);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space(2.5f);

            if (GUILayout.Button($"Build All", GUILayout.Height(20f)))
            {
                MainManager.RunBuild();
            }

            EditorGUILayout.Space(2.5f);

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = prevColor;
        }

        private static void DrawSelectedSequenceData()
        {
            _scrollPosSequence = EditorGUILayout.BeginScrollView(_scrollPosSequence);

            if (_buildList == null)
            {
                _buildList = BuildDataReordableList.Create(Settings.builds, OnBuildAdd, "Builds");
                _buildList.onSelectCallback += OnBuildSelectionChanged;
                _buildList.index = 0;
            }

            _buildList.DoLayoutList();

            EditorGUILayout.EndScrollView();
        }

        private static void DrawConfiguredBuilds()
        {
            EditorGUILayout.BeginVertical();
            
            Settings.generalScriptingDefineSymbols = EditorGUILayout.TextField("Scripting Define Symbols Default", Settings.generalScriptingDefineSymbols);
            Settings.isNeedZip = EditorGUILayout.Toggle("Need zip", Settings.isNeedZip);
            Settings.isReleaseBuild = EditorGUILayout.Toggle("Is Release build", Settings.isReleaseBuild);

            EditorGUILayout.EndVertical();
        }

        private static void DrawPathData()
        {
            Settings.outputRoot = EditorGUILayout.TextField("Output root", Settings.outputRoot);
            Settings.middlePath = EditorGUILayout.TextField("Middle path", Settings.middlePath);
            Settings.dirPathForPostProcess = EditorGUILayout.TextField("Dir path for process", Settings.dirPathForPostProcess);
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