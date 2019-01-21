using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace V02 {
    public class MainRoadGeneratorV01 : BaseRoad {
        public GameObject roadBranch;
        private int branchDistance;
        private bool canBranch;
        private int minLength;
        private int length;
        private int curStreetBranchDistance;


        // Use this for initialization
        protected override void Start() {
            InitSettings();
            InitLaserPosition();
            InitLineRenderer();
            debugPos = this.gameObject.transform.position;
            settings.currentMainRoads++;
            length = Random.Range(minLength, maxLenght);
        }

        //Get all settings from settingsObject
        override protected void InitSettings() {
            settings = SettingsObject.Instance;
            if (!settings.mainRoadGenerators.Contains(this)) {
                settings.mainRoadGenerators.Add(this);
            }
            noise = settings.MR_noise;
            roadCrossingSnapDistance = settings.MR_roadCrossingSnapDistance;
            angle = settings.MR_angle;
            laserDistance = settings.MR_laserDistance;
            populationMap = settings.populationMap;
            waterMap = settings.waterMap;
            canBranch = settings.MR_canBranch;
            minBranchDistance = settings.MR_minimalBranchDistance;
            branchProb = settings.MR_branchProbability;
            maxLenght = settings.MR_MaxRoadLength;
            roadColor = settings.MR_roadColor;
        }

        //Create new highwayGenerator
        public void InitBranch(Vector3 rot, Vector3 pos) {
            LateStart();
            this.transform.position = pos;

            List<int> x = new List<int>(); //Position
            List<int> z = new List<int>(); //Position
            List<float> y = new List<float>(); //Rotation

            this.transform.eulerAngles = new Vector3(rot.x, rot.y + settings.MR_branchAngle, rot.z);
            x.Add(Mathf.RoundToInt(laserPos.transform.position.x));
            z.Add(Mathf.RoundToInt(laserPos.transform.position.z));
            y.Add(rot.y + settings.MR_branchAngle);

            this.transform.eulerAngles = new Vector3(rot.x, rot.y - settings.MR_branchAngle, rot.z);
            x.Add(Mathf.RoundToInt(laserPos.transform.position.x));
            z.Add(Mathf.RoundToInt(laserPos.transform.position.z));
            y.Add(rot.y - settings.MR_branchAngle);

            Vector3 bestOnPopMap = PopulationConstraints(x, z, y);
            bool waterConstraints = WaterConstraints(Mathf.RoundToInt(bestOnPopMap.z), Mathf.RoundToInt(bestOnPopMap.x));
            this.transform.eulerAngles = new Vector3(rot.x, bestOnPopMap.y, rot.z);
        }


        protected override void Update() {
            if (curLenght < maxLenght) {
                if (this.transform.position.x > 0 && this.transform.position.x < settings.populationMap.width && this.transform.position.z > 0 && this.transform.position.z < settings.populationMap.height && curLenght < length) {
                    if(settings.R_minimalBranchDistance < curStreetBranchDistance) {
                        NewStreet();
                        curStreetBranchDistance = 0;
                    }
                    else {
                        curStreetBranchDistance++;
                    }
                    BuildNewRoad();
                    curLenght++;
                }
                else {
                    DestroyGenerator();
                }
            }
            else {
                DestroyGenerator();
            }
        }

        //Creates new MainRoad
        private void BuildNewRoad() {
            if (curLenght < length) {
                GetBestPosition(); //<- Calls Constraints and BuildRoad();
                DrawNewRoad("MainRoad", 2.5f, 0);
            }
            else {
                DestroyGenerator();
            }
        }

        //Place new point if all tests are correct or disable this object
        protected override void BuildRoad(bool noWater, Vector3 position) {
            if (noWater == true) {
                Vector2 p = RoadCrossing(position);

                //If p is not 0,0, that means a crossing has been found or created on this point (Kills RoadGenerator)
                if (p != new Vector2(0, 0)) {
                    this.transform.position = new Vector3(p.x, 0, p.y);
                    this.transform.eulerAngles = new Vector3(0, 0, 0);
                    DestroyGenerator();
                    return;
                }
                //Set new occupied position in a Quad
                foreach (Quad quad in settings.quads) {
                    if (position.x < quad.quadPosition.x && position.z < quad.quadPosition.y) {
                        quad.occupied.Add(new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z)));
                        break; //stay in own Quad
                    }
                }

                this.transform.position = new Vector3(position.x, 0, position.z);
                this.transform.eulerAngles = new Vector3(0, position.y, 0);
                //Reset Debug LineRenderer
                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, laserPos.transform.position);

                if(settings.currentMainRoads < settings.maxMainRoads) {
                    if (canBranch) {
                        branchDistance++;
                        int spawnNumber = Random.Range(0, 100 + 1);
                        if (branchDistance > minBranchDistance && spawnNumber < branchProb) {
                            GameObject branch = Instantiate(BranchPrfab, null);
                            branch.GetComponent<MainRoadGeneratorV01>().InitBranch(this.transform.eulerAngles, this.transform.position);
                            branchDistance = 0;
                        }
                    }
                }
            }
            else {
                DestroyGenerator();
            }
        }

        //Create new street
        public void NewStreet() {
            foreach (Quad quad in settings.quads) {
                if (this.transform.position.x < quad.quadPosition.x && this.transform.position.z < quad.quadPosition.y) {
                    //If quad is found, loop through all occupied possitions
                    foreach (Vector2 q in quad.occupiedHighway) {
                        //if there is a occupied possition found within range of our current position of X and Z
                        if (this.transform.position.x < q.x + (settings.highwayClearance) && this.transform.position.x > q.x - (settings.highwayClearance)) {
                            if (this.transform.position.z < q.y + (settings.highwayClearance) && this.transform.position.z > q.y - (settings.highwayClearance)) {
                                return;
                            }
                        }
                    }
                }
            }

             Vector3 x = this.transform.eulerAngles;

            //Kijk naar +~90 graden
            float rot = Random.Range(settings.R_minAngle, settings.R_maxAngle);
            this.transform.Rotate(new Vector3(0, rot, 0));
            if (populationMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale < settings.R_minPopulation) {
                Instantiate(roadBranch, this.transform.position, this.transform.rotation, null);
            }
            this.transform.eulerAngles = x;

            //Kijk naar -~90 graden
            this.transform.Rotate(new Vector3(0, -rot, 0));
            if (populationMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale < settings.R_minPopulation) {
                Instantiate(roadBranch, this.transform.position, this.transform.rotation, null);
            }
            this.transform.eulerAngles = x;
        }

        //Sets generator to be removed in the SettingsObject
        protected override void DestroyGenerator() {
            settings.mainRoadGenerators.Remove(this);
            base.DestroyGenerator();
        }
    }
}
