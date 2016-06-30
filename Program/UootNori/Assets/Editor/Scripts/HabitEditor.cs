using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;

namespace PatternSystem
{
    [CustomEditor(typeof(HabitAgent))]
	public class HabitEditor : Editor {

        HabitAgent _habit;

        List<UnityEngine.Object> _editorPrefabs = new List<UnityEngine.Object>();

		void OnEnable () {

            _habit = target as HabitAgent;
		}

        void Init()
        {
            string[] GUIDs = AssetDatabase.FindAssets("Triger t:Prefab", new string[] {"Assets/Resources/PatternPrefabs/Triger"});

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
			EditorGUILayout.BeginVertical ();
			{
                if (_habit.transform.parent != null)
                {
                    _habit._comment = EditorGUILayout.TextField("Habit Comment", _habit._comment);
                    if (GUILayout.Button("Add Triger"))
                    {
                        if (_editorPrefabs.Count == 0)
                            Init();
                        
                        Object habit = _editorPrefabs[0];
                        GameObject triger = PrefabUtility.InstantiatePrefab(habit) as GameObject;
                        triger.name = triger.name.Replace("(clone)", "");
                        triger.transform.SetParent(_habit.transform);
                    }

                    List<TrigerAgent> trigers = _habit.CollectTriger();
                    if(trigers.Count > 0)
                    {
                        int index = 0;
                        int trigerSelected = 0;
                        List<string> trigerNames = new List<string>();
                        HashSet<string> overlap = new HashSet<string>();
                        foreach(TrigerAgent triger in trigers)
                        {
                            if (triger.TrigerName == _habit.ActiveTriger)
                                trigerSelected = index;
                            trigerNames.Add(triger.TrigerName);
                            ++index;
                            if (overlap.Contains(triger.TrigerName))
                                Debug.Log("Overlap Triger Name !! -> " + triger.TrigerName);
                            else
                                overlap.Add(triger.TrigerName);
                        }

                        EditorGUILayout.BeginHorizontal();
                        trigerSelected = EditorGUILayout.Popup("Choice Active Triger", trigerSelected, trigerNames.ToArray());
                        EditorGUILayout.EndHorizontal();
                        _habit.ActiveTriger = trigerNames[trigerSelected];
                    }
                    else
                    {
                        GUILayout.TextArea("Add Triger");
                    }
                }
                else
                {
                    GUILayout.Box("absolute Habit to GameObject child");
                }
			}
			EditorGUILayout.EndVertical ();

			if (GUI.changed)
				EditorUtility.SetDirty(target);
		}
	}
}


