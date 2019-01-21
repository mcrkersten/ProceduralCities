using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace v01 {
    public class BranchV01 : MonoBehaviour {

        [HideInInspector]
        public Vector3 curPos = new Vector3();

        private int lastDirection;
        private Vector3 lastPosition;
        private int direction;
        private GameObject roads;

        public GameObject cube;
        public int maxRoadLength;

        private void Start() {
            StreetGenerator.OnCreateRoad += MakeRoad;
            this.transform.position = curPos;
            roads = GameObject.Find("Roads");
        }

        private void MakeRoad() {

            int roadSize = Random.Range(5, (maxRoadLength + 1));
            direction = Random.Range(1, 5);

            while (lastDirection == direction) {
                direction = Random.Range(1, 5);
            }

            for (int i = 0; i < roadSize; i++) {
                lastPosition = this.transform.position;
                switch (direction) {
                    case 1:
                        this.transform.Translate(Vector3.forward);
                        lastDirection = 2;
                        break;
                    case 2:
                        this.transform.Translate(Vector3.back);
                        lastDirection = 1;
                        break;
                    case 3:
                        this.transform.Translate(Vector3.right);
                        lastDirection = 4;
                        break;
                    case 4:
                        this.transform.Translate(Vector3.left);
                        lastDirection = 3;
                        break;
                }


                curPos = this.transform.position;
                StreetGenerator.Instance.roadPositions.Add(curPos);

                List<Vector3> toTest = new List<Vector3> {
                new Vector3(curPos.x + 1, curPos.y, curPos.z),
                new Vector3(curPos.x - 1, curPos.y, curPos.z),
                new Vector3(curPos.x, curPos.y, curPos.z + 1),
                new Vector3(curPos.x, curPos.y, curPos.z - 1)
            };

                toTest.Remove(lastPosition);
                toTest.Remove(-lastPosition);

                Instantiate(cube, this.transform.position, transform.rotation, roads.transform);

                if (curPos.x > GroundSystem.Instance.xMeshSize / 2 || curPos.x < -GroundSystem.Instance.xMeshSize / 2 || curPos.z > GroundSystem.Instance.zMeshSize / 2 || curPos.z < -GroundSystem.Instance.zMeshSize / 2) {
                    StreetGenerator.OnCreateRoad -= MakeRoad;
                    Destroy(this.gameObject);
                    return;
                }
                if (StreetGenerator.Instance.TestPosition(toTest)) {
                    StreetGenerator.OnCreateRoad -= MakeRoad;
                    Destroy(this.gameObject);
                    return;
                }
                StreetGenerator.Instance.RoadBranch(this.transform.position, false);
            }
        }
    }
}

