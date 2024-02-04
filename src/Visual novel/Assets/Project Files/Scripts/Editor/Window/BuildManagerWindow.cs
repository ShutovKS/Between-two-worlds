using System.Linq;
using Editor.BuildManager.Core;
using Editor.Changelog;
using UnityEditor;
using UnityEditor.Build;
using UnityEditorInternal;
using UnityEngine;

namespace Editor.Window
{
    public class BuildManagerWindow : EditorWindow
    {
        //Need Assets in path, cuz used by AssetDatabase.CreateAsset
        private const string SETTINGS_DEFAULT_PATH = "Assets/Editor/Setting/BuildSequences.asset";

        private const string SETTINGS_PATH_KEY = "BuildManagerWindow.SettingsPath";

        private static string _settingsPath;
        private static BuildManagerSettings _settings;

        private static bool _globalDataFoldout = true;

        private static ChangelogData _changelog;
        private static bool _changelogFoldout = false;
        private static Vector2 _scrollPosChangelog = Vector2.zero;

        private static Vector2 _scrollPosSequence = Vector2.zero;
        private static bool _postBuildFoldout = false;

        private static ReorderableList _sequenceList;
        private static ReorderableList _buildList;

        [MenuItem("File/Build Manager", false, 205)]
        public static void ShowWindow()
        {
            _sequenceList = null;
            _buildList = null;
            GetWindow(typeof(BuildManagerWindow), false, "Builds", true);

            LoadSettings();
        }

        private void OnGUI()
        {
            if (_settings == null)
            {
                LoadSettings();
            }

            if (_changelog == null)
            {
                LoadChangelog();
            }

            DrawGlobalBuildData();
            DrawChangelogInfo();

            if (!_changelogFoldout)
            {
                DrawBuildButtons();

                EditorGUILayout.Space(20);
                _scrollPosSequence = EditorGUILayout.BeginScrollView(_scrollPosSequence);

                DrawSequenceList();
                DrawSelectedSequenceData();

                EditorGUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.HelpBox("Close changelog to acess build data", MessageType.Warning);
            }

            EditorUtility.SetDirty(_settings);
        }

        #region Main Drawers

        private void DrawGlobalBuildData()
        {
            _globalDataFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_globalDataFoldout, "Global data");
            if (_globalDataFoldout)
            {
                ++EditorGUI.indentLevel;

                //Versions
                PlayerSettings.companyName = EditorGUILayout.TextField("Company Name", PlayerSettings.companyName);
                PlayerSettings.productName = EditorGUILayout.TextField("Product Name", PlayerSettings.productName);
                PlayerSettings.bundleVersion = EditorGUILayout.TextField("Version", PlayerSettings.bundleVersion);
                PlayerSettings.Android.bundleVersionCode = EditorGUILayout.IntField("Android bundle version",
                    PlayerSettings.Android.bundleVersionCode);

                //Defines
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                _settings.scriptingDefineSymbols =
                    EditorGUILayout.TextField("Scripting Defines", _settings.scriptingDefineSymbols);
                if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
                {
                    var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                    var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                    PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, _settings.scriptingDefineSymbols);
                }

                EditorGUILayout.EndHorizontal();

                --EditorGUI.indentLevel;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            if (_globalDataFoldout)
            {
                EditorGUILayout.Space(20);
            }
        }

        private static void DrawChangelogInfo()
        {
            var oldChangelogFoldoutValue = _changelogFoldout;
            _changelogFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_changelogFoldout, "Changelog");
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (_changelogFoldout)
            {
                _scrollPosChangelog = EditorGUILayout.BeginScrollView(_scrollPosChangelog /*, GUILayout.Height(800f)*/);
                ++EditorGUI.indentLevel;

                EditorGUILayout.LabelField("Readme");
                _changelog.readme = EditorGUILayout.TextArea(_changelog.readme);
                GUILayout.Space(10f);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Changelog file:", GUILayout.Width(100));
                if (GUILayout.Button($"Add version"))
                {
                    _changelog.versions.Add(new ChangelogData.ChangelogVersionEntry());
                }

                EditorGUILayout.EndHorizontal();

                for (var i = _changelog.versions.Count - 1; i >= 0; --i)
                {
                    var version = _changelog.versions[i];

                    EditorGUILayout.BeginHorizontal();
                    version.foldout =
                        EditorGUILayout.BeginFoldoutHeaderGroup(version.foldout, $"{version.version} - {version.date}");
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    if (GUILayout.Button($"Remove version", GUILayout.Width(100)))
                    {
                        _changelog.versions.RemoveAt(i);
                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (string.IsNullOrEmpty(version.version))
                    {
                        version.version = PlayerSettings.bundleVersion;
                    }

                    if (string.IsNullOrEmpty(version.date))
                    {
                        version.date = System.DateTime.Now.ToShortDateString();
                    }

                    if (version.foldout)
                    {
                        ++EditorGUI.indentLevel;

                        EditorGUILayout.BeginHorizontal();
                        version.version = EditorGUILayout.TextField("Version", version.version);
                        if (GUILayout.Button($"Curr", GUILayout.Width(70)))
                        {
                            version.version = PlayerSettings.bundleVersion;
                        }

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        version.date = EditorGUILayout.TextField("Date", version.date);
                        if (GUILayout.Button($"Now", GUILayout.Width(70)))
                        {
                            version.date = System.DateTime.Now.ToShortDateString();
                        }

                        EditorGUILayout.EndHorizontal();

                        version.updateName = EditorGUILayout.TextField("Update name", version.updateName);
                        version.descriptionText = EditorGUILayout.TextField("Description", version.descriptionText);

                        EditorGUILayout.LabelField("Notes: ");

                        ++EditorGUI.indentLevel;
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Type", GUILayout.Width(150));
                        EditorGUILayout.LabelField("Scope", GUILayout.Width(125));
                        EditorGUILayout.LabelField("Community", GUILayout.Width(100));
                        EditorGUILayout.LabelField("Description");
                        EditorGUILayout.EndHorizontal();

                        for (var j = 0; j < version.notes.Count; ++j)
                        {
                            var versionNote = version.notes[j];
                            EditorGUILayout.BeginHorizontal();

                            var newType = (ChangelogData.ChangelogEntryType)EditorGUILayout.EnumPopup(versionNote.type,
                                GUILayout.Width(150));
                            var newScope = (ChangelogData.ChangelogEntryScope)EditorGUILayout.EnumPopup(
                                versionNote.scope,
                                GUILayout.Width(150));
                            versionNote.isCommunityFeedback = EditorGUILayout.Toggle(versionNote.isCommunityFeedback,
                                GUILayout.Width(70));
                            versionNote.text = EditorGUILayout.TextField(versionNote.text);

                            if (versionNote.type != newType || versionNote.scope != newScope)
                            {
                                versionNote.type = newType;
                                versionNote.scope = newScope;
                                version.notes = version.notes.OrderBy(note => note.type).ThenBy(note => note.scope)
                                    .ToList();
                                return;
                            }

                            if (GUILayout.Button($"-", GUILayout.Width(25)))
                            {
                                version.notes.RemoveAt(j);
                                return;
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        --EditorGUI.indentLevel;

                        if (GUILayout.Button($"Add note"))
                        {
                            version.notes.Add(new ChangelogData.ChangelogNoteEntry());
                        }

                        --EditorGUI.indentLevel;
                    }

                    EditorGUILayout.Space(10);
                }

                --EditorGUI.indentLevel;
                EditorGUILayout.EndScrollView();
            }

            if (oldChangelogFoldoutValue != _changelogFoldout)
            {
                ChangelogData.SaveChangelog(_changelog);

#if GAME_TEMPLATE
		TemplateGameManager.Instance.buildNameString = changelog.GetLastVersion().GetVersionHeader();;
		TemplateGameManager.Instance.productName = PlayerSettings.productName;
		EditorUtility.SetDirty(TemplateGameManager.Instance);
#endif
            }

            if (_changelogFoldout)
            {
                EditorGUILayout.Space(20);
            }
        }

        private static void DrawBuildButtons()
        {
            if ((_settings != null ? _settings.sequences?.Count ?? 0 : 0) == 0)
            {
                return;
            }

            var enabledSequence = _settings.sequences?.Count(sequence => sequence.isEnabled);

            if (enabledSequence == 0)
            {
                return;
            }

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0.773f, 0.345098f, 0.345098f);

            EditorGUILayout.LabelField("Start build sequence(they red not becouse error, but becouse build " +
                                       "stuck your pc if you accidentaly press it)");
            EditorGUILayout.LabelField("Don't forget to manually download new version of polyglot localization " +
                                       "if you want to update it");

            EditorGUILayout.BeginHorizontal();
            if (_settings.sequences != null)
            {
                for (var i = 0; i < _settings.sequences.Count; ++i)
                {
                    var sequence = _settings.sequences[i];

                    if (i != 0 && i % 3 == 0)
                    {
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                    }

                    if (sequence.isEnabled && GUILayout.Button($"Build {sequence.editorName}"))
                    {
                        BuildManager.Core.BuildManager.RunBuildSequnce(_settings, sequence, _changelog);
                    }
                }
            }

            EditorGUILayout.EndHorizontal();

            GUI.backgroundColor = prevColor;
        }

        private static void DrawSequenceList()
        {
            if (_sequenceList == null)
            {
                PredefinedBuildConfigs.Init();
                _sequenceList =
                    BuildSequenceReordableList.Create(_settings.sequences, OnSequenceAdd, "Builds sequences");
                _sequenceList.onSelectCallback += OnSequenceSelectionChanged;
                _sequenceList.index = 0;
            }

            _sequenceList.DoLayoutList();

            if (0 <= _sequenceList.index && _sequenceList.index < _sequenceList.count)
            {
                var selected = _settings.sequences[_sequenceList.index];

                EditorGUILayout.BeginHorizontal();
                selected.scriptingDefineSymbolsOverride = EditorGUILayout.TextField("Defines sequence override",
                    selected.scriptingDefineSymbolsOverride);
                if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
                {
                    var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                    var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                    var concat = string.Concat(_settings.scriptingDefineSymbols, ";",
                        selected.scriptingDefineSymbolsOverride);
                    PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, concat);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawSelectedSequenceData()
        {
            EditorGUILayout.Space(20);

            if (_sequenceList.index < 0 || _sequenceList.index >= _settings.sequences.Count)
            {
                _buildList = null;
                return;
            }

            if (_buildList == null)
            {
                _buildList = BuildDataReordableList.Create(_settings.sequences[_sequenceList.index].builds, OnBuildAdd,
                    "Builds");
                _buildList.onSelectCallback += OnBuildSelectionChanged;
                _buildList.index = 0;
            }

            _buildList.DoLayoutList();

            if (_buildList.index < 0 || _buildList.index >= _settings.sequences[_sequenceList.index].builds.Count)
            {
                return;
            }

            var selected = _settings.sequences[_sequenceList.index].builds[_buildList.index];

            var obj = new SerializedObject(_settings);

            selected.isPassbyBuild = EditorGUILayout.Toggle("Is Passby build", selected.isPassbyBuild);
            selected.isReleaseBuild = EditorGUILayout.Toggle("Is Release build", selected.isReleaseBuild);

            EditorGUILayout.Space(20);
            selected.outputRoot = EditorGUILayout.TextField("Output root", selected.outputRoot);
            selected.middlePath = EditorGUILayout.TextField("Middle path", selected.middlePath);
            selected.dirPathForPostProcess = EditorGUILayout.TextField("Dir path", selected.dirPathForPostProcess);

            EditorGUILayout.BeginHorizontal();
            selected.scriptingDefineSymbolsOverride =
                EditorGUILayout.TextField("Defines build override", selected.scriptingDefineSymbolsOverride);
            if (GUILayout.Button($"Set defines", GUILayout.Width(100f)))
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup);
                var concat = string.Concat(_settings.scriptingDefineSymbols, ";",
                    _settings.sequences[_sequenceList.index].scriptingDefineSymbolsOverride, ";",
                    selected.scriptingDefineSymbolsOverride);
                PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, concat);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        #endregion

        #region Loaders

        private static void LoadSettings()
        {
            _settingsPath = PlayerPrefs.GetString(SETTINGS_PATH_KEY, "");
            _settings = null;

            //Find path. Try to load settings
            if (!string.IsNullOrEmpty(_settingsPath))
            {
                _settings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(_settingsPath);
                if (_settings == null)
                {
                    _settingsPath = null;
                }
            }

            //No path, or cant locate asset at path. Try to find settings in assets.
            if (string.IsNullOrEmpty(_settingsPath))
            {
                var guids = AssetDatabase.FindAssets("t:BuildManagerSettings", new[] { "Assets" });
                if (guids.Length >= 2)
                {
                    Debug.LogError("[BuildManagerWindow]. 2+ BuildManagerSettings exist. " +
                                   "Consider on using only 1 setting. The first one will be used.");
                }

                if (guids.Length != 0)
                {
                    _settingsPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    PlayerPrefs.SetString(SETTINGS_PATH_KEY, _settingsPath);
                    _settings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(_settingsPath);
                }
            }

            //Cant find settings. Create new
            if (_settings == null)
            {
                var defaultSettings = AssetDatabase.LoadAssetAtPath<BuildManagerSettings>(
                    AssetDatabase.GUIDToAssetPath(
                        AssetDatabase.FindAssets("t:BuildManagerSettings", new[] { "Packages" })[0]));

                _settings = (BuildManagerSettings)CreateInstance(typeof(BuildManagerSettings));
                _settings.CloneInto(defaultSettings);

                AssetDatabase.CreateAsset(_settings, SETTINGS_DEFAULT_PATH);
                _settingsPath = SETTINGS_DEFAULT_PATH;
                PlayerPrefs.SetString(SETTINGS_PATH_KEY, SETTINGS_DEFAULT_PATH);
            }
        }

        private static void LoadChangelog()
        {
            _changelog = ChangelogData.LoadChangelog();
        }

        #endregion

        #region Callbacks

        private static void OnSequenceSelectionChanged(ReorderableList list)
        {
            _buildList = null;
        }

        private static void OnBuildSelectionChanged(ReorderableList list)
        {
        }

        private static void OnSequenceAdd(object target)
        {
            _settings.sequences.Add((target as BuildSequence)?.Clone() as BuildSequence);
        }

        private static void OnBuildAdd(object target)
        {
            _settings.sequences[_sequenceList.index].builds.Add((target as BuildData)?.Clone() as BuildData);
        }

        #endregion

        #region Helpers

        private void GuiLine(int iHeight = 1)
        {
            var rect = EditorGUILayout.GetControlRect(false, iHeight);
            rect.height = iHeight;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }

        #endregion
    }
}