using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    [CustomEditor(typeof(TrigerAgent))]
	public class TrigerEditor : AttributeEditor {

        TrigerAgent _triger;
        List<UnityEngine.Object> _editorPrefabs = new List<UnityEngine.Object>();
		void OnEnable () {

            _triger = target as TrigerAgent;

		}

        void Init()
        {
            string[] GUIDs = AssetDatabase.FindAssets("", new string[] {"Assets/Resources/PatternPrefabs/Triger/Arrange"});

            for (int index = 0; index < GUIDs.Length; index++)
            {
                string guid = GUIDs[index];
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
                _editorPrefabs.Add(asset);
            }
        }

		public override void OnInspectorGUI()
		{
            if (_editorPrefabs.Count == 0)
                Init();
            
            if (_triger.transform.parent == null)
            {
                GUILayout.Box("absolute Triger to Habit child");
                return;
            }

            if (_triger.transform.parent.name != "Habit")
            {
                GUILayout.Box("absolute Triger to Habit child");
                return;
            }

			EditorGUILayout.BeginHorizontal();
			{
				_triger.TrigerName	 = EditorGUILayout.TextField("Key Name", _triger.TrigerName);
			}
			EditorGUILayout.EndHorizontal ();
            for(int i = 0; i < _editorPrefabs.Count; ++i)
            {
                string btnlabel = "Add --> ";
                btnlabel += _editorPrefabs[i].name;
				if(GUILayout.Button(btnlabel))
				{
                    Object attribute = _editorPrefabs[i];
					GameObject triger = PrefabUtility.InstantiatePrefab(attribute) as GameObject;
					triger.transform.SetParent (_triger.transform);
				}
			}

			if (GUI.changed)
				EditorUtility.SetDirty(_triger);

		}
	}	
}


