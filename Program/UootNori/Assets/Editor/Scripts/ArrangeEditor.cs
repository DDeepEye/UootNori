using UnityEngine;
using UnityEditor;
using PatternSystem;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{
    [CustomEditor(typeof(ArrangeAgent))]
	public class ArrangeEditor : AttributeEditor 
	{
        ArrangeAgent _arrange;
        List<UnityEngine.Object> _editorPrefabs = new List<UnityEngine.Object>();

		void OnEnable () {
            _arrange = target as ArrangeAgent;
            _attribute = _arrange;
		}

        void Init()
        {
            string[] GUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] {"Assets/Resources/PatternPrefabs/Triger/Arrange"});

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

            if (_arrange.transform.parent == null)
            {
                return;
            }
            
            if (_arrange.transform.parent != null)
            {
                TrigerAgent ta = _arrange.transform.parent.GetComponent<TrigerAgent>();
                if(ta == null)
                {
                    ArrangeAgent ag = _arrange.transform.parent.GetComponent<ArrangeAgent>();
                    if (ag == null)
                        return;
                }   
            }
            
			EditorGUILayout.BeginVertical ();
			{
				_arrange._type = (Arrange.ArrangeType) EditorGUILayout.EnumPopup ("Arrange type", _arrange._type);
				_arrange._repeat = EditorGUILayout.IntField ("Repeat count",_arrange._repeat);
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginVertical ();
			{
                
                for(int i = 0; i < _editorPrefabs.Count; ++i)
				{
					string btnlabel = "Add --> ";
                    btnlabel += _editorPrefabs[i].name;

					if(GUILayout.Button(btnlabel))
					{
                        Object attribute = _editorPrefabs[i];
						GameObject triger = PrefabUtility.InstantiatePrefab(attribute) as GameObject;
                        triger.transform.SetParent(_arrange.transform);
					}
				}
			}

			EditorGUILayout.EndVertical ();

			if (GUI.changed)
				EditorUtility.SetDirty(_attribute);
		}
	}

}



