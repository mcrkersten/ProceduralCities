using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace V02 {
    //[ExecuteInEditMode]
    public class SettingsMenu : EditorWindow {

        private SettingsObject settings;
        private int minLimit = 85;
        private int maxLimit = 95;
        private int tab = 0; 

        [MenuItem("Window/CityGenerator")]
        private static void ShowWindow() {        
            GetWindow<SettingsMenu>("City Settings");
        }

        private void OnGUI() {
            settings = SettingsObject.Instance;
            

            if (settings.editorStart) {
                IsGenerating();
                //this.Focus();
            }
            else if (settings.editorEnd) {
                DoneGenerating();
                //this.Focus();
            }

            else {
                tab = GUILayout.Toolbar(tab, new string[] { "Highways", "Main Roads", "Streets" });
                switch (tab) {
                    case 0:
                        Highway();
                        break;

                    case 1:
                        MainRoad();
                        break;


                    case 2:
                        Street();
                        break;
                }
            }
            
        }

        private void Highway() {
            var style = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Space(10);
            GUILayout.Label("Texture Maps", style);

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 95, 60, 200, 120));
            EditorGUILayout.BeginHorizontal();
            settings.populationMap = TextureField("Population", settings.populationMap);
            settings.waterMap = TextureField("Water", settings.waterMap);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            GUILayout.Space(115);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 190, 210, 140));
            GUILayout.Label("Basic Highway Settings", style);
            EditorGUILayout.BeginHorizontal();
            settings.H_angle = VariableIntField("Bend Angle", "The maximum angle a highway is allowed to bend", settings.H_angle, 35);
            settings.H_noise = VariableIntField("Bend Noise", "creates more random road pattern", settings.H_noise, 5);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            settings.H_laserDistance = VariableFloatField("Section Length", "The Length of new generated highway section", settings.H_laserDistance);
            settings.H_roadLength = VariableIntField("Highway Length", "The max Length a whole highway may reach", settings.H_roadLength, 150);
            EditorGUILayout.EndHorizontal();
            settings.H_roadColor = ColorField("Road Color", "Debug line color", settings.H_roadColor);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 65, 330, 200, 100));
            EditorGUILayout.BeginHorizontal();
            settings.H_canBranch = VariableBoolField("Highway can branch", settings.H_canBranch);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();


            if (settings.H_canBranch) {
                GUILayout.Space(160);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                minSize = new Vector2(250, 535);
                maxSize = new Vector2(250, 535);
                GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 370, 210, 165));
                GUILayout.Label("Branch Settings", style);
                settings.H_branchProbability = IntSliderField("Branch probability", settings.H_branchProbability);
                ProgressBar(settings.H_branchProbability / 100.0f, "Branch probability: " + settings.H_branchProbability + "%");

                EditorGUILayout.BeginHorizontal();
                settings.H_branchAngle = VariableIntField("Branch Angle", "The Angle of the new road against the current road", settings.H_branchAngle, 90);
                settings.H_minimalBranchDistance = VariableIntField("Dist branches", "The minimal amount of highway pieces between a new intersection", settings.H_minimalBranchDistance, 50);
                EditorGUILayout.EndHorizontal();

                settings.maxHighways = VariableIntField("Max highways", "The maximum amount of individual highways", settings.maxHighways, 30);
                GUILayout.EndArea();
                GUILayout.Space(150);

            }
            else {
                GUILayout.Space(170);
                minSize = new Vector2(250, 375);
                maxSize = new Vector2(250, 375);
            }

            if (GUILayout.Button("Build City")) {
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }

        private void MainRoad() {
            var style = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Space(10);
            GUILayout.Label("Main road Settings", style);

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 60, 210, 160));
            EditorGUILayout.BeginHorizontal();
            settings.MR_angle = VariableIntField("Bend Angle", "The max amount a new section can rotate", settings.MR_angle, 20);
            settings.MR_noise = VariableIntField("Bend Noise", "creates more random road pattern", settings.MR_noise, 15);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            settings.MR_MinRoadLength = VariableIntField("Min Length", "Minimum amount of roadsections a single generator can create.", settings.MR_MinRoadLength, 25);
            settings.MR_MaxRoadLength = VariableIntField("Max Length", "Maximum amount of roadsections a single generator can create.", settings.MR_MaxRoadLength, 50);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            settings.MR_laserDistance = VariableFloatField("Section Length", "Maximum amount of roadsections a single generator can create.", settings.MR_laserDistance);
            settings.maxMainRoads = VariableIntField("Max main Roads", "The Maximum allowed Main roads", settings.maxMainRoads, 100);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
            settings.MR_roadColor = ColorField("Road Color", "Minimum amount of roadsections a single generator can create.", settings.MR_roadColor);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, 230, 200, 100));
            EditorGUILayout.BeginHorizontal();
            settings.MR_canBranch = VariableBoolField("Road can branch", settings.MR_canBranch);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (settings.MR_canBranch) {
                GUILayout.Space(195);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                minSize = new Vector2(250, 407);
                maxSize = new Vector2(250, 407);
                GUILayout.BeginArea(new Rect((Screen.width / 2) - 110, 270, 220, 165));
                GUILayout.Label("Branch Settings", style);
                settings.MR_branchProbability = IntSliderField("Branch probability", settings.MR_branchProbability);
                ProgressBar(settings.MR_branchProbability / 100.0f, "Branch probability: " + settings.MR_branchProbability + "%");
                EditorGUILayout.BeginHorizontal();
                settings.MR_branchAngle = VariableIntField("Branch Angle", "The Angle of the new road", settings.MR_branchAngle, 90);
                settings.MR_minimalBranchDistance = VariableIntField("Dist branches", "The minimum distance between branches", settings.MR_minimalBranchDistance, 50);
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
            else {
                GUILayout.Space(75);
                minSize = new Vector2(250, 270);
                maxSize = new Vector2(250, 270);
            }

            GUILayout.Space(120);
            if (GUILayout.Button("Build City")) {
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }

        private void Street() {
            

            var style = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Space(10);
            GUILayout.Label("Basic Street Settings", style);

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 60, 210, 120));
            EditorGUILayout.BeginHorizontal();
            settings.R_angle = VariableIntField("Bend angle", "The amount a road can bend without being a intersection", settings.R_angle, 35);
            settings.R_noise = VariableIntField("Bend noise", "creates more random road pattern", settings.R_noise, 15);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            settings.R_minPopulation = VariableFloatField("Minimal pop", "Minimal population for a road to spawn", settings.R_minPopulation);
            settings.R_laserDistance = VariableFloatField("Road Length", "The Length of new generated road section", settings.R_laserDistance);
            EditorGUILayout.EndHorizontal();

            settings.R_roadColor = ColorField("Road Color", "Debug line color", settings.R_roadColor);
            GUILayout.EndArea();

            GUILayout.Space(115);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 190, 210, 150));
            GUILayout.Label("Intersection Settings", style);

            EditorGUILayout.BeginHorizontal();
            settings.R_minAngle = VariableIntField("Min Angle", "The minimal angle a intersaction has on it parent", settings.R_minAngle, 1000);
            settings.R_maxAngle = VariableIntField("Max Angle", "The maximal angle a intersaction has on it parent", settings.R_maxAngle, 1000);
            EditorGUILayout.EndHorizontal();
            AngleVisual(settings.R_maxAngle, settings.R_minAngle);

            GUILayout.Space(10);
            settings.R_branchProbability = IntSliderField("Branch probability", settings.R_branchProbability);
            ProgressBar(settings.R_branchProbability / 100.0f, "Branch probability: " + settings.R_branchProbability + "%");
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect((Screen.width / 2) - 50, 330, 200, 100));
            EditorGUILayout.BeginHorizontal();
            settings.buildBuildings = VariableBoolField("Build buildings", settings.buildBuildings);
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            if (settings.buildBuildings) {
                GUILayout.Space(160);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                GUILayout.Label("Building Settings", style);
                minSize = new Vector2(250, 455);
                maxSize = new Vector2(250, 455);

                GUILayout.BeginArea(new Rect((Screen.width / 2) - 105, 395, 210, 120));
                EditorGUILayout.BeginHorizontal();
                settings.homeThreshold = VariableFloatField("Tower Threshold", "The minimum population needed for a tower", settings.homeThreshold);
                settings.pointTowerThreshold = VariableFloatField("P-twr Threshold", "The minimum population needed for a point-tower", settings.pointTowerThreshold);
                EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
                GUILayout.Space(45);
            }
            else {
                GUILayout.Space(170);
                minSize = new Vector2(250, 378);
                maxSize = new Vector2(250, 378);
            }

            
            if (GUILayout.Button("Build City")) {
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }

        private void IsGenerating() {
            var style = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Space(10);
            GUILayout.Label("Currently Generating", style);

            if (GUILayout.Button("Stop Generating")) {
                UnityEditor.EditorApplication.isPaused = true;
                settings.editorStart = false;
                settings.editorEnd = true;
            }
        }

        private void DoneGenerating() {
            var style = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };
            GUILayout.Space(10);
            GUILayout.Label("Done Generating", style);

            if (GUILayout.Button("Save City as OBJ")) {
                DoExport(true);
            }

            if (GUILayout.Button("Cancel")) {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        private static Texture2D TextureField(string name, Texture2D texture) {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.UpperCenter,
                fixedWidth = 90
            };
            GUILayout.Label(name, style);
            texture = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(90), GUILayout.Height(90));
            GUILayout.EndVertical();
            return texture;
        }

        private static Color ColorField(string name, string tooltip, Color color) {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.UpperCenter,
                fixedWidth = 210
            };

            GUIContent content = new GUIContent(name, tooltip);
            GUILayout.Label(content, style);
            color = EditorGUILayout.ColorField(color, GUILayout.Width(210));
            GUILayout.EndVertical();
            return color;
        }

        private static int VariableIntField(string name, string tooltip, int value, int maxRecommend) {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.UpperCenter,
                fixedWidth = 100
            };

            if (value > maxRecommend) {
                style.normal.textColor = Color.red;
                tooltip = "Max value of " + maxRecommend + " is recommended";
            }

            GUIContent content = new GUIContent(name, tooltip);
            GUILayout.Label(content, style);
            value = EditorGUILayout.IntField(value, GUILayout.Width(100));
            GUILayout.EndVertical();
            return value;
        }

        private static float VariableFloatField(string name, string tooltip, float value) {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.UpperCenter,
                fixedWidth = 100
            };
            GUIContent content = new GUIContent(name, tooltip);
            GUILayout.Label(content, style);
            value = EditorGUILayout.FloatField(value, GUILayout.Width(100));
            GUILayout.EndVertical();
            return value;
        }

        private static bool VariableBoolField(string name, bool value) {
            GUILayout.BeginVertical();
            value = GUILayout.Toggle(value, name, GUI.skin.toggle);
            GUILayout.EndVertical();
            return value;
        }

        private static int IntSliderField(string name, int value) {
            GUILayout.BeginVertical();
            value = EditorGUILayout.IntSlider(value, 0, 100);
            GUILayout.EndVertical();
            return value;
        }

        void ProgressBar(float value, string label) {
            // Get a rect for the progress bar using the same margins as a textfield:
            Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }

        void AngleVisual(float min, float max) {

            if (min > 10 && max > 10) {
                if (settings.R_minAngle > settings.R_maxAngle) {
                    settings.R_maxAngle++;
                }
                if (settings.R_maxAngle < settings.R_minAngle) {
                    settings.R_minAngle--;
                }
                if (settings.R_maxAngle > maxLimit) {
                    settings.R_maxAngle = maxLimit;
                }
                if (settings.R_minAngle < minLimit) {
                    settings.R_minAngle = minLimit;
                }
            }
            EditorGUILayout.MinMaxSlider(ref max, ref min, minLimit, maxLimit);
        }

        private static void DoExport(bool makeSubmeshes) {
            string meshName = "GeneratedCity";
            string fileName = EditorUtility.SaveFilePanel("Export .obj file", "", meshName, "obj");

            ObjExporterScript.Start();

            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + meshName + ".obj"
                                + "\n#" + System.DateTime.Now.ToLongDateString()
                                + "\n#" + System.DateTime.Now.ToLongTimeString()
                                + "\n#-------"
                                + "\n\n");

            Transform t = SettingsObject.Instance.gameObject.transform;

            Vector3 originalPosition = t.position;
            t.position = Vector3.zero;

            if (!makeSubmeshes) {
                meshString.Append("g ").Append(t.name).Append("\n");
            }
            meshString.Append(ProcessTransform(t, makeSubmeshes));

            WriteToFile(meshString.ToString(), fileName);

            t.position = originalPosition;

            ObjExporterScript.End();
            Debug.Log("Exported Mesh: " + fileName);
        }

        private static string ProcessTransform(Transform t, bool makeSubmeshes) {
            StringBuilder meshString = new StringBuilder();

            meshString.Append("#" + t.name
                            + "\n#-------"
                            + "\n");

            if (makeSubmeshes) {
                meshString.Append("g ").Append(t.name).Append("\n");
            }

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf) {
                meshString.Append(ObjExporterScript.MeshToString(mf, t));
            }

            for (int i = 0; i < t.childCount; i++) {
                meshString.Append(ProcessTransform(t.GetChild(i), makeSubmeshes));
            }

            return meshString.ToString();
        }

        private static void WriteToFile(string s, string filename) {
            using (StreamWriter sw = new StreamWriter(filename)) {
                sw.Write(s);
            }
        }
    }

    public class ObjExporterScript {
        private static int StartIndex = 0;

        public static void Start() {
            StartIndex = 0;
        }
        public static void End() {
            StartIndex = 0;
        }


        public static string MeshToString(MeshFilter mf, Transform t) {
            Vector3 s = t.localScale;
            Vector3 p = t.localPosition;
            Quaternion r = t.localRotation;


            int numVertices = 0;
            Mesh m = mf.sharedMesh;
            if (!m) {
                return "####Error####";
            }

            StringBuilder sb = new StringBuilder();

            foreach (Vector3 vv in m.vertices) {
                Vector3 v = t.TransformPoint(vv);
                numVertices++;
                sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            sb.Append("\n");
            foreach (Vector3 nn in m.normals) {
                Vector3 v = r * nn;
                sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, -v.z));
            }
            sb.Append("\n");
            foreach (Vector3 v in m.uv) {
                sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
            }
            for (int material = 0; material < m.subMeshCount; material++) {
                sb.Append("\n");

                int[] triangles = m.GetTriangles(material);
                for (int i = 0; i < triangles.Length; i += 3) {
                    sb.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n",
                        triangles[i] + 1 + StartIndex, triangles[i + 1] + 1 + StartIndex, triangles[i + 2] + 1 + StartIndex));
                }
            }

            StartIndex += numVertices;
            return sb.ToString();
        }
    }
}



