using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Assets.Libraries.UnityHelpers.Editor.Tools
{
    public class ScriptableObjectUtility : EditorWindow
    {
        private bool isOpen;

        [MenuItem("Unity Helpers/Create Asset")]
        public static void CreateAsset()
        {
            GetWindow<ScriptableObjectUtility>();
        }

        //[MenuItem("UnityHelpers/Create Asset 2")]
        //public static void CreateAsset2()
        //{
        //    CreateAsset<LevelsData>();
        //}    

        void OnGUI()
        {
            if (!isOpen)
            {
                int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
                EditorGUIUtility.ShowObjectPicker<MonoScript>(null, false, "", controlID);
                isOpen = true;
            }
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                var script = (MonoScript)EditorGUIUtility.GetObjectPickerObject();
                if(script!=null)
                {
                    var path = AssetDatabase.GetAssetOrScenePath(script);
                    var classname = Path.GetFileNameWithoutExtension(path);
                    var type = AppDomain.CurrentDomain.GetAssemblies()
                                   .SelectMany(x => x.GetTypes())
                                   .FirstOrDefault(x => x.Name == classname);
                    if (type == null) Debug.LogError("Could not find type with class name: "+classname);
                    else CreateAsset(type);
                    Close();
                }
            }
        }

        public static void CreateAsset<T>() where T : ScriptableObject
        {
            CreateAsset(typeof(T));
        }

        /// <summary>    
        /// This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// Borrowed from: http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
        /// </summary>
        public static void CreateAsset(Type t)
        {
            if (!t.IsSubclassOf(typeof(ScriptableObject))) throw new Exception("Asset must a be a scriptable object!");

            var asset = ScriptableObject.CreateInstance(t);

            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")
            {
                path = "Assets";
            }
            else if (Path.GetExtension(path) != "")
            {
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/"+t.Name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
    }
}
