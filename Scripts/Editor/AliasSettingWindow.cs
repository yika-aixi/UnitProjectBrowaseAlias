using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CabinIcarus.ProjectBrowserAliasTools
{
    static class SerializationUtil
    {
        public static string ToString<T>(T value)
        {
            throw new NotImplementedException("Please implement serialization yourself");
        }

        public static T ToValue<T>(string str)
        {
            throw new NotImplementedException("Please implement serialization yourself");
        }
    }
    
    public class AliasSettingWindow : EditorWindow
    {
        class AliasInfo:IEquatable<AliasInfo>
        {
            public string Path { get; }
            public string Alias { get; }

            public AliasInfo(string path, string @alias)
            {
                Path = path;
                Alias = alias;
            }

            public AliasInfo():this(string.Empty,string.Empty)
            {
            }

            public AliasInfo(string path):this(path,string.Empty)
            {
            }

            public bool Equals(AliasInfo other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Path == other.Path;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AliasInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Path != null ? Path.GetHashCode() : 0) * 397;
                }
            }
        }
        
        private static List<AliasInfo> _aliasInfos;

        private static GameObject[] _LastSelects;

        [MenuItem("Icarus/Assets/Alias Setting")]
        private static void ShowWindow()
        {
            var window = GetWindow<AliasSettingWindow>();
            window.titleContent = new GUIContent("File/Folder Alias Setting");
            window.Show();
            Selection.selectionChanged += () => { _LastSelects = Selection.gameObjects; };
        }

        private const string _AliasInfosKey = "Alias Setting -> Paths";
        
        private void OnGUI()
        {
            if (GUILayout.Button("Add Alias Item"))
            {
                if (_LastSelects == null || _LastSelects.Length == 0)
                {
                    _aliasInfos.Add(new AliasInfo());
                }
                else
                {
                    foreach (var obj in _LastSelects)
                    {
                        var path = AssetDatabase.GetAssetPath(obj);

                        if (string.IsNullOrEmpty(path))
                        {
                            continue;
                        }
                    
                        _aliasInfos.Add(new AliasInfo(path));
                    }
                }
                
                Save();
            }

            _drawItem();
        }

        private Vector2 _pos;

        private void _drawItem()
        {
            if (_aliasInfos.Count == 0)
            {
                return;
            }
            
            var temp = _aliasInfos.ToArray();
            
            _pos = EditorGUILayout.BeginScrollView(_pos);
            EditorGUILayout.BeginVertical("box");
            {
                for (var i = 0; i < temp.Length; i++)
                {
                    var aliasInfo = _aliasInfos[i];

                    EditorGUILayout.BeginHorizontal();
                    {
                        {
                            string path;
                            string alias;
                            EditorGUI.BeginChangeCheck();
                            {
                                path = EditorGUILayout.DelayedTextField(aliasInfo.Path);
                                alias = EditorGUILayout.DelayedTextField(aliasInfo.Alias);
                            }
                            if (EditorGUI.EndChangeCheck())
                            {
                                _aliasInfos[i] = new AliasInfo(path,alias);
                                Save();
                            }
                        }

                        if (GUILayout.Button(EditorGUIUtility.FindTexture("d_P4_DeletedLocal")))
                        {
                            _aliasInfos.RemoveAt(i);
                            Save();
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        
        [InitializeOnLoadMethod]
        static void _Init()
        {
            var str = EditorUserSettings.GetConfigValue(_AliasInfosKey);

            try
            {
                _aliasInfos = SerializationUtil.ToValue<List<AliasInfo>>(str);
            }
            finally
            {
                if (_aliasInfos == null)
                {
                    _aliasInfos = new List<AliasInfo>();
                    Save();
                }
            }
        }
        
        public static void Save()
        {
            var str = SerializationUtil.ToString(_aliasInfos);

            EditorUserSettings.SetConfigValue(_AliasInfosKey,str);
        }

        public static string GetAlias(string path)
        {
            var alias = _aliasInfos.FirstOrDefault(x => x.Path == path);

            return alias?.Alias;
        }

        public static string GetAliasGuid(string guid)
        {
            return GetAlias(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}