using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V02 {
    [ExecuteInEditMode]
    public class Gizmo : MonoBehaviour {

        [SerializeField]
        private Mesh singleSpawn;
        [SerializeField]
        private Mesh doubleSpawn;
        [SerializeField]
        private Mesh tripleSpawn;
        [SerializeField]
        private Mesh quadSpawn;
        public int spawnPoints;

        private SettingsObject settings;

        private void Start() {
            settings = SettingsObject.Instance;
        }

        private void OnDrawGizmos() {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = new Color32(214, 48, 49, 255);

            if (spawnPoints == 1) {
                Gizmos.DrawMesh(singleSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }

            else if (spawnPoints == 2) {
                Gizmos.DrawMesh(doubleSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }

            else if (spawnPoints == 3) {
                Gizmos.DrawMesh(tripleSpawn, new Vector3(0, 1.5f, -1), Quaternion.identity, new Vector3(25, 25, 25));
            }
            else {
                Gizmos.DrawMesh(quadSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }
            Gizmos.DrawWireSphere(new Vector3(0, 1.5f, 0), 7.5f);
        }
    }
}

