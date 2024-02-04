using System.Collections.Generic;
using Editor.BuildManager.Core;
using UnityEditor;
using UnityEngine;

namespace Editor.Window
{
    public class BuildDataReordableList
    {
        public static UnityEditorInternal.ReorderableList Create(List<BuildData> configsList,
            GenericMenu.MenuFunction2 menuItemHandler, string header)
        {
            var reorderableList = new UnityEditorInternal.ReorderableList(
                configsList, typeof(BuildData), true, false, true, true)
            {
                elementHeight = EditorGUIUtility.singleLineHeight + 4,
                drawHeaderCallback = rect => { EditorGUI.LabelField(rect, header); },
                drawElementCallback = (position, index, isActive, isFocused) =>
                {
                    const float ENABLED_WIDTH = 15f;
                    const float BUILD_TARGET_GROUP_WIDTH = 125f;
                    const float BUILD_TARGET_WIDTH = 150f;
                    const float MINBUILD_OPTIONS_WIDTH = 100f;
                    const float SPACE = 10f;

                    var buildOptionsWidth =
                        position.width - SPACE * 6 - ENABLED_WIDTH - BUILD_TARGET_WIDTH - BUILD_TARGET_GROUP_WIDTH;
                    if (buildOptionsWidth < MINBUILD_OPTIONS_WIDTH)
                    {
                        buildOptionsWidth = MINBUILD_OPTIONS_WIDTH;
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
                        var label = /*"Standalone/" +*/
                            BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
                        menu.AddItem(new GUIContent(label), false, menuItemHandler, config);
                    }

                    menu.AddSeparator("");

                    foreach (var config in PredefinedBuildConfigs.AndroidData)
                    {
                        var label = /*"Android/" +*/
                            BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
                        menu.AddItem(new GUIContent(label), false, menuItemHandler, config);
                    }

                    menu.AddSeparator("");

                    foreach (var config in PredefinedBuildConfigs.WebData)
                    {
                        var label = /*"WebGL/"+ */
                            BuildManager.Core.BuildManager.ConvertBuildTargetToString(config.target);
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