using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V02 {
    [CustomEditor(typeof(Gizmo))]
    public class GizmoMenu : Editor {

        bool oneSpawn;
        bool twoSpawn;
        bool threeSpawn;
        bool fourSpawn;

        void OnSceneGUI() {

            var titleStyle = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };

            var nameStyle = new GUIStyle(GUI.skin.label) {
                fontSize = 10,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };

            Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(50, (Screen.height / 3), 200, Screen.height - (Screen.height / 5)));
                GUI.color = new Color32(223, 230, 233, 150);


                GUI.Box(new Rect(new Vector2(0, 0), new Vector2(200, 150)), GUIContent.none);
                GUI.color = new Color32(223, 230, 233, 255);
                GUILayout.Space(10);
                titleStyle.normal.textColor = Color.black;
                GUILayout.Label("Spawn-point settings", titleStyle);

                GUI.Box(new Rect(new Vector2(100 - 62.5f, 40), new Vector2(125, 90)), GUIContent.none);
                GUILayout.BeginArea(new Rect(100 - 50, 50, 100, 100));

                    //Road spawn direction amount
                    
                    GUILayout.Label("spawn directions", nameStyle);
                    GUILayout.BeginHorizontal();
                    
                    if (Selection.activeGameObject.GetComponent<Gizmo>() != null) {
                        if (GUILayout.Toggle(oneSpawn, "1", "Button") || Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints == 1) {
                            oneSpawn = true;
                            twoSpawn = false;
                            threeSpawn = false;
                            fourSpawn = false;
                            Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 1;
                        }

                        if (GUILayout.Toggle(twoSpawn, "2", "Button") || Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints == 2) {
                            oneSpawn = false;
                            twoSpawn = true;
                            threeSpawn = false;
                            fourSpawn = false;
                            Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 2;
                        }

                        EditorGUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        if (GUILayout.Toggle(threeSpawn, "3", "Button") || Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints == 3) {
                            oneSpawn = false;
                            twoSpawn = false;
                            threeSpawn = true;
                            fourSpawn = false;
                            Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 3;
                        }

                        if (GUILayout.Toggle(fourSpawn, "4", "Button") || Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints == 4) {
                            oneSpawn = false;
                            twoSpawn = false;
                            threeSpawn = false;
                            fourSpawn = true;
                            Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 4;
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                GUILayout.EndArea();
            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}
