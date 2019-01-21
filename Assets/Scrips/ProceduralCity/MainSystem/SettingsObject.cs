using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Text;

namespace V02 {
    public class SettingsObject : MonoBehaviour {

        private static SettingsObject instance = null;
        public static SettingsObject Instance
        {
            get {
                if (instance == null) {
                    // This is where the magic happens.
                    instance = FindObjectOfType(typeof(SettingsObject)) as SettingsObject;
                }

                // If it is still null, create a new instance
                if (instance == null) {
                    GameObject i = new GameObject("Setings");
                    i.AddComponent(typeof(SettingsObject));
                    instance = i.GetComponent<SettingsObject>();
                }
                return instance;
            }
        }

        //BASE-ROAD
        ///////////////////////////////////////////////////////////////////////////////
        public Texture2D populationMap;
        public Texture2D waterMap;

        //HIGHWAY
        ///////////////////////////////////////////////////////////////////////////////
        [Header("Basic Highway Setings")]
        public float H_roadCrossingSnapDistance = 1.5f;
        public int H_noise;
        public int H_angle;
        public int H_roadLength;
        public float H_laserDistance;
        public bool H_canBranch;
        public Color H_roadColor;
        public int H_branchProbability;
        public int H_branchAngle;
        public int H_minimalBranchDistance;
        public int maxHighways;
        public int currentHighways;

        //MAIN-ROAD
        ///////////////////////////////////////////////////////////////////////////////
        [Header("MainRoad Setings")]
        public float MR_roadCrossingSnapDistance = 1.25f;
        public int MR_noise;
        public int MR_angle;
        public int MR_MaxRoadLength;
        public int MR_MinRoadLength;
        public float MR_laserDistance;
        public bool MR_canBranch;
        public Color MR_roadColor;
        public int MR_branchProbability;
        public int MR_branchAngle;
        public int MR_minimalBranchDistance;
        public int maxMainRoads;
        public int currentMainRoads;

        //STREET
        ///////////////////////////////////////////////////////////////////////////////
        [Header("Street Settings")]
        public float R_roadCrossingSnapDistance = 1.2f;
        public float highwayClearance = 3;
        public int R_noise;
        public int R_angle;
        public int R_minAngle = 89;
        public int R_maxAngle = 91;
        public float R_minPopulation;
        public float R_laserDistance;
        public int R_minimalBranchDistance;
        public int R_branchProbability;
        public Color R_roadColor;
        public bool buildBuildings;
        public float pointTowerThreshold;
        public float homeThreshold;
        [HideInInspector]
        public int maxGeneratedStreets = 7;


        //UNTIL
        ///////////////////////////////////////////////////////////////////////////////
        private bool systemIsRunning = false;
        public List<Quad> quads;
        public List<Vector2> occupiedXY;
        public List<Vector2> existingCrossing;
        public List<float> existingCrossingYrot;

        public List<GameObject> roads = new List<GameObject>();
        public List<GameObject> newRoads = new List<GameObject>();
        public List<GameObject> removeRoads = new List<GameObject>();
        public int buildingAge;

        //GENERATORS
        ///////////////////////////////////////////////////////////////////////////////
        public List<HighwayGeneratorV02> highwayGenerators = new List<HighwayGeneratorV02>();
        public List<MainRoadGeneratorV01> mainRoadGenerators = new List<MainRoadGeneratorV01>();
        public List<StreetGeneratorV01> streetGenerator = new List<StreetGeneratorV01>();

        //EDITOR
        ///////////////////////////////////////////////////////////////////////////////
        public bool editorStart = false;
        public bool editorEnd = false;
        public Camera camera;

        private void Awake() {
            editorStart = true;
            InitQuads();
        }

        private void Update() {
            if (mainRoadGenerators.Count == 0 && highwayGenerators.Count == 0 && !systemIsRunning) {
                systemIsRunning = true;
                StartCoroutine(AntiRaceCondition());
            }
        }

        private IEnumerator AntiRaceCondition() {
            for (int i = 0; i < roads.Count; i++) {
                roads[i].GetComponent<StreetGeneratorV01>().BuildLoop();
                if (i != roads.Count - 1) {
                    i++;
                    roads[i].GetComponent<StreetGeneratorV01>().BuildLoop();
                }
                yield return null;
            }
            NextRound();
        }

        private void NextRound() {
            foreach (GameObject i in removeRoads) {
                roads.Remove(i);
                Destroy(i);
            }
            removeRoads.Clear();
            roads.AddRange(newRoads);
            newRoads.Clear();
            if (roads.Count != 0) {
                StartCoroutine(AntiRaceCondition());
            }
            else {
                editorEnd = true;
                editorStart = false;
                print("Done");
                UnityEditor.EditorApplication.isPaused = true;
            }
        }

        private void InitQuads() {
            GameObject quadParent = new GameObject("Quads");
            quadParent.transform.position = new Vector3(0, 0, 0);

            int width = (populationMap.width / 10);
            int height = (populationMap.height / 10);

            int w = width;
            int h = height;

            for (int i = 0; i < 20; i++) {
                CreateQuad(w, h, quadParent);
                for (int x = 0; x < 19; x++) {
                    w = w + width;
                    CreateQuad(w, h, quadParent);
                }
                w = width;
                h = h + height;
            }
        }

        private void CreateQuad(int w, int h, GameObject parent) {
            GameObject q = new GameObject("Quad" + w + " " + h);
            q.transform.parent = parent.transform;
            q.transform.position = new Vector3(w, 0, h);
            Quad qw = q.AddComponent<Quad>();
            qw.quadPosition = new Vector2(w, h);
            quads.Add(q.GetComponent<Quad>());
        }
    }
}