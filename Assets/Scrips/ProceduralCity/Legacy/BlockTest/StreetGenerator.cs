using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace v01 {
    public class StreetGenerator : MonoBehaviour {

        public delegate void CreateRoad();
        public static event CreateRoad OnCreateRoad;
        public GameObject branch;
        private int probabilityGrow = 1;
        private int probability = 1;

        public List<Vector3> roadPositions = new List<Vector3>();

        private static StreetGenerator instance = null;
        public static StreetGenerator Instance
        {
            get {
                if (instance == null) {
                    // This is where the magic happens.
                    //  FindObjectOfType(...) returns the first PlayerController object in the scene.
                    instance = FindObjectOfType(typeof(StreetGenerator)) as StreetGenerator;
                }

                // If it is still null, create a new instance
                if (instance == null) {
                    GameObject obj = new GameObject("PlayerController");
                    instance = obj.AddComponent(typeof(StreetGenerator)) as StreetGenerator;
                    Debug.Log("Could not locate an StreetGenerator object.  PlayerController was Generated Automaticly.");
                }
                return instance;
            }
        }

        private void Start() {
            RoadBranch(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), true);
            RoadBranch(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), true);
            RoadBranch(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), true);
            RoadBranch(new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50)), true);

        }


        private void Update() {
            if (Input.anyKey) {
                if (OnCreateRoad != null) {
                    OnCreateRoad();
                }
            }
        }

        public void RoadBranch(Vector3 pos, bool start) {
            if (!start) {
                int a = Random.Range(0, probability);
                probability += probabilityGrow;

                if (a == 1) {
                    GameObject br = Instantiate(branch);
                    br.GetComponent<BranchV01>().curPos = pos;
                }
            }
            else {
                GameObject br = Instantiate(branch);
                br.GetComponent<BranchV01>().curPos = pos;
            }
        }

        public bool TestPosition(List<Vector3> positions) {
            foreach (Vector3 p in positions) {
                if (roadPositions.Contains(p)) {
                    return true;
                }
            }
            return false;
        }
    }
}

