using System.Collections.Generic;
using Editor.BuildManager.Core;
using Editor.ScriptingDefineSymbols;
using UnityEditor;
using UnityEngine;

namespace Editor.Window
{
    public class BuildDataReordableList
    {
        private const float ENABLED_WIDTH = 15f;
        private const float BUILD_TARGET_GROUP_WIDTH = 100f;
        private const float BUILD_TARGET_WIDTH = 150f;
        private const float BUILD_TARGET_ADDONS_USED = 150f;
        private const float MIN_BUILD_OPTIONS_WIDTH = 100f;
        private const float SPACE = 5f;

        public static UnityEditorInternal.ReorderableList Create(List<BuildData> configsList,
            GenericMenu.MenuFunction2 menuItemHandler, string header)
        {
            var reorderableList = new UnityEditorInternal.ReorderableList(configsList, typeof(BuildData),
                true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight + 4,
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, header); },
                drawElementCallback = (position, index, isActive, isFocused) =>
                {
                    var buildOptionsWidth = position.width - ENABLED_WIDTH - BUILD_TARGET_WIDTH -
                                            BUILD_TARGET_GROUP_WIDTH - BUILD_TARGET_ADDONS_USED - SPACE * 5;

                    if (buildOptionsWidth < MIN_BUILD_OPTIONS_WIDTH)
                    {
                        buildOptionsWidth = MIN_BUILD_OPTIONS_WIDTH;
                    }

                    var data = configsList[index];

                    position.y += 2;
                    position.height -= 4;

                    position.x += SPACE;
                    position.width = ENABLED_WIDTH;
                    data.isEnabled = EditorGUI.Toggle(position, data.isEnabled);
                    EditorGUI.BeginDisabledGroup(!data.isEnabled);

                    position.x += position.width + SPACE;
                    position.width = BUILD_TARGET_GROUP_WIDTH;
                    data.targetGroup = (BuildTargetGroup)EditorGUI.EnumPopup(position, data.targetGroup);

                    position.x += position.width + SPACE;
                    position.width = BUILD_TARGET_WIDTH;
                    data.target = (BuildTarget)EditorGUI.EnumPopup(position, data.target);

                    position.x += position.width + SPACE;
                    position.width = BUILD_TARGET_ADDONS_USED;
                    data.addonsUsed = (AddonsUsedType)EditorGUI.EnumFlagsField(position, data.addonsUsed);

                    position.x += position.width + SPACE;
                    position.width = buildOptionsWidth;
                    data.options = (BuildOptions)EditorGUI.EnumFlagsField(position, data.options);

                    EditorGUI.EndDisabledGroup();
                },

                onAddDropdownCallback = (buttonRect, list) =>
                {
                    var menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Custom"), false, menuItemHandler, new BuildData());

                    menu.AddSeparator("");

                    foreach (var config in PredefinedBuildConfigs.StandaloneData)
                    {
                        /*"Standalone/" +*/
                        var label = BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
                        menu.AddItem(new GUIContent(label), false, menuItemHandler, config);
                    }

                    menu.AddSeparator("");

                    foreach (var config in PredefinedBuildConfigs.AndroidData)
                    {
                        /*"Android/" +*/
                        var label = BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
                        menu.AddItem(new GUIContent(label), false, menuItemHandler, config);
                    }

                    menu.AddSeparator("");

                    foreach (var config in PredefinedBuildConfigs.WebData)
                    {
                        /*"WebGL/"+ */
                        var label = BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
                        menu.AddItem(new GUIContent(label), false, menuItemHandler, config);
                    }

                    menu.AddSeparator("");

                    menu.ShowAsContext();
                }
            };

            return reorderableList;
        }
    }
}