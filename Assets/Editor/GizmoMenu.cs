using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V02 {
    [CustomEditor(typeof(Gizmo))]
    public class GizmoMenu : Editor {

        void OnSceneGUI() {
            var textStyle = new GUIStyle(GUI.skin.label) {
                fontSize = 15,
                fixedHeight = 22,
                alignment = TextAnchor.UpperCenter,
            };


            Handles.BeginGUI();

            GUILayout.BeginArea(new Rect(10, (Screen.height / 5) / 3, 200, Screen.height - (Screen.height / 5)));
                GUI.color = new Color32(223, 230, 233, 100);


                GUI.Box(new Rect(new Vector2(0, 0), new Vector2(200, 500)), GUIContent.none);
                GUI.color = new Color32(223, 230, 233, 255);
                GUILayout.Space(10);
                textStyle.normal.textColor = Color.black;
                GUILayout.Label("Spawn-point settings", textStyle);

                GUILayout.BeginArea(new Rect(100 - 50, 50, 100, 100));
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("1")) {
                        Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 1;
                    }
                    if (GUILayout.Button("2")) {
                        Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 2;
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("3")) {
                        Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 3;
                    }
                    if (GUILayout.Button("4")) {
                        Selection.activeGameObject.GetComponent<Gizmo>().spawnPoints = 4;
                    }
                    EditorGUILayout.EndHorizontal();
                GUILayout.EndArea();
            GUILayout.EndArea();

            Handles.EndGUI();
        }
    }
}
