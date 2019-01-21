using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace V02 {
    public class HighwayGeneratorV02 : BaseRoad {
        public GameObject mainRoadBranchPrefab;
        private int branchDistanceHW;
        private int branchDistanceMR;
        private bool canBranch;
        private int curStreetBranchDistance;
        
        protected override void Start() {
            InitSettings();
            InitLaserPosition();
            InitLineRenderer();
            debugPos = this.gameObject.transform.position;
            settings.currentHighways++;
        }

        //Get all settings from settingsObject
        override protected void InitSettings()  {
            settings = SettingsObject.Instance;

            if (!settings.highwayGenerators.Contains(this)) {
                settings.highwayGenerators.Add(this);
            }          
            noise = settings.H_noise;
            roadCrossingSnapDistance = settings.H_roadCrossingSnapDistance;
            angle = settings.H_angle;
            laserDistance = settings.H_laserDistance;
            populationMap = settings.populationMap;
            waterMap = settings.waterMap;
            canBranch = settings.H_canBranch;
            minBranchDistance = settings.H_minimalBranchDistance;
            branchProb = settings.H_branchProbability;
            maxLenght = settings.H_roadLength;
            roadColor = settings.H_roadColor;
        }

        //Create new highwayGenertator
        public void InitBranch(Vector3 rot, Vector3 pos) {
            LateStart();
            this.transform.position = pos;

            List<int> x = new List<int>(); //Position
            List<int> z = new List<int>(); //Position
            List<float> y = new List<float>(); //Rotation

            this.transform.eulerAngles = new Vector3(rot.x, rot.y + settings.H_branchAngle, rot.z);
            x.Add(Mathf.RoundToInt(laserPos.transform.position.x));
            z.Add(Mathf.RoundToInt(laserPos.transform.position.z));
            y.Add(rot.y + settings.H_branchAngle);

            this.transform.eulerAngles = new Vector3(rot.x, rot.y - settings.H_branchAngle, rot.z);
            x.Add(Mathf.RoundToInt(laserPos.transform.position.x));
            z.Add(Mathf.RoundToInt(laserPos.transform.position.z));
            y.Add(rot.y - settings.H_branchAngle);

            Vector3 bestOnPopMap = PopulationConstraints(x, z, y);
            bool waterConstraints = WaterConstraints(Mathf.RoundToInt(bestOnPopMap.z), Mathf.RoundToInt(bestOnPopMap.x));
            this.transform.eulerAngles = new Vector3(rot.x, bestOnPopMap.y, rot.z);
        }

        protected override void Update() {
            if (curLenght < maxLenght) {
                if (this.transform.position.x > 0 && this.transform.position.x < settings.populationMap.width && this.transform.position.z > 0 && this.transform.position.z < settings.populationMap.height) {
                    GetBestPosition(); //<- Calls Constraints and BuildRoad();
                    DrawNewRoad("HighWay", 5, 1);
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
                        quad.occupiedHighway.Add(new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z)));
                        quad.occupied.Add(new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z)));
                        break; //stay in own Quad
                    }
                }

                this.transform.position = new Vector3(position.x, 0, position.z);
                this.transform.eulerAngles = new Vector3(0, position.y, 0);
                //Reset Debug LineRenderer
                lr.SetPosition(0, this.transform.position);
                lr.SetPosition(1, laserPos.transform.position);

                if(settings.currentHighways < settings.maxHighways) {
                    if (canBranch) {
                        branchDistanceHW++;
                        int spawnNumber1 = Random.Range(0, 100 + 1);
                        if (branchDistanceHW > minBranchDistance && spawnNumber1 < branchProb) {
                            GameObject branch = Instantiate(BranchPrfab, null);
                            branch.GetComponent<HighwayGeneratorV02>().InitBranch(this.transform.eulerAngles, this.transform.position);
                            branchDistanceHW = 0;
                        }
                    }
                }
                if (settings.currentMainRoads < settings.maxMainRoads) {
                    NewMainRoad();
                    branchDistanceMR++;

                }
            }
            else {
                DestroyGenerator();
            }
        }

        //Creates new MainRoad
        public void NewMainRoad() {
            int spawnNumber = Random.Range(0, 100 + 1);
            if (branchDistanceMR > (settings.MR_minimalBranchDistance) && spawnNumber < settings.MR_branchProbability) {
                GameObject branch = Instantiate(mainRoadBranchPrefab, null);
                branch.GetComponent<MainRoadGeneratorV01>().InitBranch(this.transform.eulerAngles, this.transform.position);
                branchDistanceMR = 0;
            }
        }

        //Sets generator to be removed in the SettingsObject
        protected override void DestroyGenerator() {
            settings.highwayGenerators.Remove(this);
            base.DestroyGenerator();
        }
    }
}
