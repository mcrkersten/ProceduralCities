using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V02 {
    [ExecuteInEditMode]
    public class Gizmo : MonoBehaviour {

        [SerializeField]
        public Mesh singleSpawn, doubleSpawn, tripleSpawn, quadSpawn;

        [SerializeField]
        public GameObject singleSpawnP, doubleSpawnP, tripleSpawnP, quadSpawnP;
        [HideInInspector]
        public int spawnPoints;

        private GameObject currentSpawn, test;

        private void OnDrawGizmos() {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = new Color32(214, 48, 49, 255);

            if (spawnPoints == 1) {
                if(currentSpawn != singleSpawnP) {
                    DestroyImmediate(test);
                    test = Instantiate(singleSpawnP, this.transform);
                    currentSpawn = singleSpawnP;
                }
                Gizmos.DrawMesh(singleSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }

            else if (spawnPoints == 2) {
                if (currentSpawn != doubleSpawnP) {
                    DestroyImmediate(test);
                    test = Instantiate(doubleSpawnP, this.transform);
                    currentSpawn = doubleSpawnP;
                }
                Gizmos.DrawMesh(doubleSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }

            else if (spawnPoints == 3) {
                if (currentSpawn != tripleSpawnP) {
                    DestroyImmediate(test);
                    test = Instantiate(tripleSpawnP, this.transform);
                    currentSpawn = tripleSpawnP;
                }
                Gizmos.DrawMesh(tripleSpawn, new Vector3(0, 1.5f, -1), Quaternion.identity, new Vector3(25, 25, 25));
            }
            else {
                if (currentSpawn != quadSpawnP) {
                    DestroyImmediate(test);
                    test = Instantiate(quadSpawnP, this.transform);
                    currentSpawn = quadSpawnP;
                }
                Gizmos.DrawMesh(quadSpawn, new Vector3(0, 1.5f, 0), Quaternion.identity, new Vector3(25, 25, 25));
            }
            Gizmos.DrawWireSphere(new Vector3(0, 1.5f, 0), 7.5f);
        }
    }
}

