using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PatternSystem
{   
	public class HabitAgent : MonoBehaviour {
        public static GameObject s_listManager;


        string _activeTriger = "";
        public string ActiveTriger { get { return _activeTriger; } set { _activeTriger = value; } }

        public string _comment = "comment";

        Dictionary<string, Triger> _trigers = new Dictionary<string, Triger>();

        public HabitAgent()
        {
            
        }
        ~HabitAgent()
        {
        }

        void Start()
        {
			if (transform.parent == null)
				return;

            ///gameObject.hideFlags = HideFlags.HideInHierarchy;
			List<TrigerAgent> trigers= CollectTriger ();

			foreach (TrigerAgent triger in trigers)
			{
				AddTriger(triger.GetTriger(transform.parent.gameObject));
			}

            if(_activeTriger.Length == 0 && trigers.Count > 0)
            {
                _activeTriger = trigers[0].name;
            }

			Play (_activeTriger);
        }

        void Update()
        {
            if (_trigers.ContainsKey(_activeTriger))
            {
                _trigers[_activeTriger].Run();
            }
        }

        public void Play(string trigerKey)
        {
            _activeTriger = trigerKey;
        }

        public void AddTriger(Triger t)
        {
            _trigers.Add(t.Key, t);
        }

        public List<TrigerAgent> CollectTriger()
		{
            List<TrigerAgent> trigers = new List<TrigerAgent> ();
            int cnt = transform.childCount;
			for (int i = 0; i < cnt; ++i)
			{
				Transform t = transform.GetChild (i);
                TrigerAgent triger = t.gameObject.GetComponent<TrigerAgent>();
				if (triger != null)
				{
					trigers.Add (triger);
				}				
			}
			return trigers;
		}
	}
}

