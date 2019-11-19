//创建者:Icarus
//手动滑稽,滑稽脸
//ヾ(•ω•`)o
//https://www.ykls.app
//2019年11月02日-21:03
//Assembly-CSharp-Editor

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CabinIcarus.ProjectBrowserAliasTools
{
    //https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ProjectBrowser.cs
    [InitializeOnLoad]
    public partial class ProjectEntityAssetDes
    {
        private static Type _ProjectBrowserType;
        private static FieldInfo _TreeRect;
        private static object _ProjectBrowser;

        private static char[] _Symbols = new[] {':','/','\\','*','?','"','<','>','|'};
        
        static ProjectEntityAssetDes()
        {
            _ProjectBrowserType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.ProjectBrowser");
            EditorApplication.projectWindowItemOnGUI += _projectWindowItemOnGui;
//            EditorApplication.update += _setCustomLabel;
        }

        private static void _setCustomLabel()
        {
            //todo 更新,先return
            return;
            
            var projectBrowsers = Resources.FindObjectsOfTypeAll(_ProjectBrowserType);

            if (projectBrowsers == null)
            {
                return;
            }
            
            foreach (var projectBrowser in projectBrowsers)
            {
                _projectBrowserSetCustomLabel(projectBrowser);
            }
        }

        private static void _projectBrowserSetCustomLabel(Object projectBrowser)
        {
            //todo 更新

            _ProjectBrowser = projectBrowser;
            
            var m_ListAreaF =
                _ProjectBrowserType.GetField("m_ListArea", BindingFlags.NonPublic | BindingFlags.Instance);

            var m_ListArea = m_ListAreaF.GetValue(_ProjectBrowser);


            var m_InstanceIDToCroppedNameMapF =
                m_ListArea.GetType().GetField("m_InstanceIDToCroppedNameMap",
                    BindingFlags.NonPublic | BindingFlags.Instance);

            var m_InstanceIDToCroppedNameMap =
                (Dictionary<int, string>) m_InstanceIDToCroppedNameMapF.GetValue(m_ListArea);

            var keys = m_InstanceIDToCroppedNameMap.Keys.ToList();

            foreach (var key in keys)
            {
                var element = m_InstanceIDToCroppedNameMap[key];
                m_InstanceIDToCroppedNameMap[key] = $"11\n{element}";
            }


            //Tree 
            var m_FolderTreeF =
                _ProjectBrowserType.GetField("m_FolderTree", BindingFlags.NonPublic | BindingFlags.Instance);

            var m_FolderTree = m_FolderTreeF.GetValue(_ProjectBrowser);

            var dataF = m_FolderTree.GetType().GetProperty("data", BindingFlags.Public | BindingFlags.Instance);

            var data = dataF.GetValue(m_FolderTree);

            var rootF = data.GetType().GetProperty("root", BindingFlags.Public | BindingFlags.Instance);

            var root = (TreeViewItem) rootF.GetValue(data);

            foreach (var treeViewItem in root.children)
            {
                if (treeViewItem.displayName == "Assets")
                {
                    foreach (var item in treeViewItem.children)
                    {
//                        if (item.displayName)
//                        {
//                            
//                        }
                        item.displayName += $"{_Symbols[0]} 11";
                    }
                    break;
                }
            }

        }

        [MenuItem("Icarus/Test")]
        private static async void _test()
        {
        }

        private static void _projectWindowItemOnGui(string guid, Rect selectionrect)
        {
            var alias = AliasSettingWindow.GetAliasGuid(guid);

            if (string.IsNullOrWhiteSpace(alias))
            {
                return;
            }
            
            var aliasContent = new GUIContent(alias);
            
            Vector2 size = EditorStyles.label.CalcSize(aliasContent);
            
            var isTree = IsTreeView(selectionrect);

            if (!isTree)
            {
                IsIconSmall(ref selectionrect);
            }
    
            Rect textRect = new Rect( 
                selectionrect.x + 2f + Mathf.Max(0, (selectionrect.width - size.x) * 0.5f), 
                selectionrect.yMax - 15, 
                size.x, 20f);

            float cropWidth = selectionrect.width;

            if (isTree)
            {
                textRect.width = 50;
                cropWidth = textRect.width;
                textRect.x = selectionrect.xMax - textRect.width;
                selectionrect.y = selectionrect.y;
            }

            aliasContent.text = EditorStyles.label.CropText(aliasContent.text, cropWidth);
            
            EditorGUI.LabelField(textRect,aliasContent);

            EditorApplication.RepaintProjectWindow();
        }


        private static bool IsTreeView(Rect rect)
        {
            return rect.height <= 21f;
        }
        
        // https://github.com/PhannGor/unity3d-rainbow-folders/blob/master/Assets/Plugins/RainbowFolders/Editor/Scripts/RainbowFoldersBrowserIcons.cs
        private static bool IsIconSmall(ref Rect rect)
        {
            var isSmall = rect.width > rect.height;

            if (isSmall)
                rect.width = rect.height;
            else
                rect.height = rect.width;

            return isSmall;
        }

        
    }
}