using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace V02 {
    public class StreetGeneratorV01 : BaseRoad {
        public Material buildingMaterial;
        private int branchDistance;
        private float minimalPopulation;
        private int generatedStreets;
        private int minAngle;
        private int maxAngle;
        private float heighest = 1;
        private float homeThreshold;
        private float pointTowerThreshhold;
        private bool buildBuildings;
        private List<LineRenderer> madeRoads = new List<LineRenderer>();
        private float maxBuildingSize;
        private float minBuildingSize; 

        protected override void Start() {
            InitSettings();
            InitLaserPosition();
            InitLineRenderer();
            debugPos = this.gameObject.transform.position;
        }

        //Get all settings from settingsObject
        override protected void InitSettings() {
            settings = SettingsObject.Instance;
            settings.newRoads.Add(this.gameObject);
            noise = settings.R_noise;
            roadCrossingSnapDistance = settings.R_roadCrossingSnapDistance;
            waterMap = settings.waterMap;
            angle = settings.R_angle;
            minAngle = settings.R_minAngle;
            maxAngle = settings.R_maxAngle;
            laserDistance = settings.R_laserDistance;
            populationMap = settings.populationMap;
            minBranchDistance = settings.R_minimalBranchDistance;
            branchProb = settings.R_branchProbability;
            roadColor = settings.R_roadColor;
            minimalPopulation = settings.R_minPopulation;
            homeThreshold = settings.homeThreshold;
            pointTowerThreshhold = settings.pointTowerThreshold;
            buildBuildings = settings.buildBuildings;

            maxBuildingSize = laserDistance;
            minBuildingSize = laserDistance / 2;
        }

        //Function is used by SettingsObject. Used to counter Race Conditions
        public void BuildLoop () {
            if (this.transform.position.x > 0 && this.transform.position.x < settings.populationMap.width && this.transform.position.z > 0 && this.transform.position.z < settings.populationMap.height) {
                GetBestPosition();
                curLenght++;
                DrawNewRoad("Street",1,-1);
                branchDistance++;
            }
            else{
                DestroyGenerator();
                return;
            }
        }

        //Can now Destroy via this function
        protected override Vector3 PopulationConstraints(List<int> x, List<int> z, List<float> y) {
            heighest = 1;
            Vector3 heighestPopPos = new Vector3();
            for (int i = 0; i < x.Count; i++) {
                float heighestPopulation = populationMap.GetPixel(x[i], z[i]).grayscale;
                if (heighestPopulation < heighest) {
                    heighestPopPos = new Vector3(x[i], y[i], z[i]);
                    heighest = heighestPopulation;
                }
            }
            if(minimalPopulation < heighest) {
                DestroyGenerator();
            }
            return heighestPopPos;
        }

        //Place new point if all tests are correct or disable this object
        protected override void BuildRoad(bool noWater, Vector3 position) {
            bool corner = false;
            Vector3 lastPosition = this.transform.position;

            if (noWater == true && generatedStreets < settings.maxGeneratedStreets) {
                generatedStreets++;
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

                if (branchDistance > minBranchDistance) {
                    int spawnNumber = Random.Range(0, 100 + 1);
                    if (branchDistance > minBranchDistance && spawnNumber < branchProb) {
                        NewStreet();
                        branchDistance = 0;
                        corner = true;
                    }
                }
                if (buildBuildings) {
                    if (branchDistance == 0) {
                        BuildBuildings(lastPosition, position, true, corner);
                    }
                    else {
                        BuildBuildings(lastPosition, position, false, corner);
                    }
                }           
            }
            else {
                DestroyGenerator();
                return;
            }
        }

        //Generates base GameObject for building Meshes
        void BuildBuildings(Vector3 start, Vector3 end, bool newGen, bool corner) {
            GameObject building = new GameObject("building");
            building.transform.position = new Vector3(start.x, 0, start.z);
            building.transform.eulerAngles = new Vector3(0, end.y, 0);
            building.transform.parent = settings.transform;

            GenerateBuildingMesh(building, start, end, true, newGen, corner);
            GenerateBuildingMesh(building, start, end, false, newGen, corner);

            SingleMesh(building);
        }

        //Generates building meshes
        void GenerateBuildingMesh(GameObject streetBuilding, Vector3 start, Vector3 end, bool leftStreet, bool newGen, bool corner) {
            float offset = 0;
            float roadOffset = 1.5f;
            float cornerOrEndDecrease = 1;
            float streetLenght = (Vector2.Distance(new Vector2(start.x, start.z), new Vector2(end.x, end.z)));

            if (corner && newGen) {              
                cornerOrEndDecrease = (streetLenght / 8);
            }else if (corner) {
                cornerOrEndDecrease = (streetLenght / 4);
            }

            int divider = Random.Range(1, 3);
            float sum;

            List<float> divisions = new List<float>();
            for (int i = 0; i < divider; i++) {
                sum = Random.Range((streetLenght / divider), streetLenght);
                streetLenght -= sum;
                divisions.Add(sum);
            }

            
            //Building  type 1
            
            foreach (float d in divisions) {

                int buildingType = Random.Range(0, 20);

                //Point
                if(buildingType == 3 && d > maxBuildingSize/2 && pointTowerThreshhold < heighest) {
                    float size = d;
                    float height = Random.Range((laserDistance) + heighest, (laserDistance + heighest) * 4);


                    Vector3 p0 = new Vector3(0, 0, size);
                    Vector3 p1 = new Vector3(size, 0, size);

                    Vector3 p2 = new Vector3(size, 0, 0);
                    Vector3 p3 = new Vector3(0, 0, 0);

                    Vector3 p4 = new Vector3(0, height, size);
                    Vector3 p5 = new Vector3(size, height, size);

                    Vector3 p6 = new Vector3(size, height, 0);
                    Vector3 p7 = new Vector3(0, height, 0);

                    Vector3 p8 = new Vector3(size / 2, height + (height/4), size / 2);

                    Vector3[] vertices = new Vector3[]
                    {
	                // Left
	                p7, p4, p0, p3,
 
	                // Front
	                p4, p5, p1, p0,
 
	                // Back
	                p6, p7, p3, p2,
 
	                // Right
	                p5, p6, p2, p1,

                    p7, p4, p8,
                    p4, p5, p8,
                    p6, p7, p8,
                    p5, p6, p8,
                    };

                    //Vector3 up = Vector3.up;
                    Vector3 down = Vector3.down;
                    Vector3 front = Vector3.forward;
                    Vector3 back = Vector3.back;
                    Vector3 left = Vector3.left;
                    Vector3 right = Vector3.right;

                    Vector3[] normales = new Vector3[]
                    {
	                // Left
	                left, left, left, left,
 
	                // Front
	                front, front, front, front,
 
	                // Back
	                back, back, back, back,
 
	                // Right
	                right, right, right, right,

                    left, left, left,
                    front, front, front,
                    back, back, back,
                    right, right, right,
                    };

                    Vector2 _00 = new Vector2(0f, 0f);
                    Vector2 _10 = new Vector2(1f, 0f);
                    Vector2 _01 = new Vector2(0f, 1f);
                    Vector2 _11 = new Vector2(1f, 1f);
                    Vector3 _22 = new Vector3(.5f, .5f);

                    Vector2[] uvs = new Vector2[]
                    {
 
	                // Left
	                _11, _01, _00, _10,
                    
	                // Front
	                _11, _01, _00, _10,
 
	                // Back
	                _11, _01, _00, _10,
 
	                // Right
	                _11, _01, _00, _10,

                    _11, _01, _22,
                    _11, _01, _22,
                    _11, _01, _22,
                    _11, _01, _22,
                    };


                    int[] triangles = new int[]
                    {

	                // Left
	                3, 1, 0,
                    3, 2, 1,
 
	                // Front
	                7, 5, 4,
                    7, 6, 5,
 
	                // Back
	                11, 9, 8,
                    11, 10, 9,
 
	                // Right
	                15, 13, 12,
                    15, 14, 13,

                    16, 17, 18,
                    19, 20, 21,

                    22, 23, 24,
                    25, 26, 27,
                    };

                    Mesh tm = new Mesh {
                        vertices = vertices,
                        normals = normales,
                        triangles = triangles,
                        uv = uvs

                    }; ;

                    GameObject temp = new GameObject("temp");
                    temp.transform.parent = streetBuilding.transform;

                    temp.transform.rotation = streetBuilding.transform.rotation;

                    if (leftStreet) {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease / 2), 0, roadOffset);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, roadOffset);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, roadOffset);
                        }
                    }
                    else {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease), 0, (-roadOffset) - size);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, (-roadOffset) - size);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, (-roadOffset) - size);
                        }
                    }

                    offset += size +1f;
                    MeshFilter m = temp.AddComponent<MeshFilter>();
                    m.mesh = tm;
                }

                //House
                else if (homeThreshold < heighest && d > maxBuildingSize/2) {
                    float size = (d/2.5f);
                    float height = Random.Range((laserDistance / 2) + heighest, laserDistance + heighest);


                    Vector3 p0 = new Vector3(0, 0, size);
                    Vector3 p1 = new Vector3(size, 0, size);

                    Vector3 p2 = new Vector3(size, 0, 0);
                    Vector3 p3 = new Vector3(0, 0, 0);

                    Vector3 p4 = new Vector3(0, height/4, size);
                    Vector3 p5 = new Vector3(size, height/4, size);

                    Vector3 p6 = new Vector3(size, height/4, 0);
                    Vector3 p7 = new Vector3(0, height/4, 0);

                    Vector3 p8 = new Vector3(size / 2, (height/4) + (height/8), 0);
                    Vector3 p9 = new Vector3(size / 2, (height/4) + (height/8), size);


                    Vector3[] vertices = new Vector3[]
                    {
	                    // Left
	                    p7, p4, p0, p3,
 
	                    // Front
	                    p4, p5, p1, p0,
 
	                    // Back
	                    p6, p7, p3, p2,
 
	                    // Right
	                    p5, p6, p2, p1,
 
	                    // roof Left
	                    p8, p9, p7, p4,

                        // roof right
                        p9, p8, p6, p5,

                        p7, p6, p8,

                        p4, p5, p9
                    };

                    Vector3 up = Vector3.up;
                    Vector3 down = Vector3.down;
                    Vector3 front = Vector3.forward;
                    Vector3 back = Vector3.back;
                    Vector3 left = Vector3.left;
                    Vector3 right = Vector3.right;

                    Vector3[] normales = new Vector3[]
                    {
	                    // Left
	                    left, left, left, left,
 
	                    // Front
	                    front, front, front, front,
 
	                    // Back
	                    back, back, back, back,
 
	                    // Right
	                    right, right, right, right,
 
	                    // Roof left
	                    left, left, left, left,

                        //Roof right
                        right, right, right, right,

                        back, back, back,

                        front, front, front,
                    };

                    Vector2 _00 = new Vector2(0f, 0f);
                    Vector2 _10 = new Vector2(1f, 0f);
                    Vector2 _01 = new Vector2(0f, 1f);
                    Vector2 _11 = new Vector2(1f, 1f);

                    Vector2 _22 = new Vector2(1f, .5f);

                    Vector2[] uvs = new Vector2[]
                    {
 
	                    // Left
	                    _11, _01, _00, _10,
 
	                    // Front
	                    _11, _01, _00, _10,
 
	                    // Back
	                    _11, _01, _00, _10,
 
	                    // Right
	                    _11, _01, _00, _10,
 
	                    // Roof left
	                    _11, _01, _00, _10,

                        //Roof right
                        _11, _01, _00, _10,

                        // punt front
	                    _11, _10, _22,

                        //punt back
                        _11, _10, _22,
                    };

                    int[] triangles = new int[]
                    {
	                    // Left
	                    3, 1, 0,
                        3, 2, 1,
 
	                    // Front
	                    7, 5, 4,
                        7, 6, 5,
 
	                    // Back
	                    11, 9, 8,
                        11, 10, 9,
 
	                    // Right
	                    15, 13, 12,
                        15, 14, 13,

                        //Roof Left
                        17, 16, 18,
                        19, 17, 18,

                        ////Roof right
                        21, 20, 23,
                        23, 22, 21,

                        26,25,24,
                        27,28,29,

                    };

                    Mesh tm = new Mesh {
                        vertices = vertices,
                        normals = normales,
                        triangles = triangles,
                        uv = uvs

                    }; ;
                    GameObject temp = new GameObject("temp");
                    temp.transform.parent = streetBuilding.transform;

                    temp.transform.rotation = streetBuilding.transform.rotation;
                    if (leftStreet) {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease / 2), 0, roadOffset);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, roadOffset);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, roadOffset);
                        }
                    }
                    else {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease), 0, (-roadOffset) - size);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, (-roadOffset) - size);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, (-roadOffset) - size);
                        }
                    }

                    offset += size + 1f;
                    MeshFilter m = temp.AddComponent<MeshFilter>();
                    m.mesh = tm;     
                }

                //Block
                else if(d > maxBuildingSize/2) {
                    float size = d;
                    float height = Random.Range((laserDistance) + heighest, (laserDistance + heighest) * 3);

                    Vector3 p0 = new Vector3(0, 0, size);
                    Vector3 p1 = new Vector3(size, 0, size);

                    Vector3 p2 = new Vector3(size, 0, 0);
                    Vector3 p3 = new Vector3(0, 0, 0);

                    Vector3 p4 = new Vector3(0, (height) - size / (height / 5), size);

                    Vector3 p5 = new Vector3(size, (height) - size / (height / 5), size);
                    Vector3 p6 = new Vector3(size, (height) - size / (height / 5), 0);
                    Vector3 p7 = new Vector3(0, (height) - size / (height / 5), 0);



                    Vector3[] vertices = new Vector3[]
                    {
	                // Left
	                p7, p4, p0, p3,
 
	                // Front
	                p4, p5, p1, p0,
 
	                // Back
	                p6, p7, p3, p2,
 
	                // Right
	                p5, p6, p2, p1,
 
	                // Top
	                p7, p6, p5, p4
                    };

                    Vector3 up = Vector3.up;
                    Vector3 down = Vector3.down;
                    Vector3 front = Vector3.forward;
                    Vector3 back = Vector3.back;
                    Vector3 left = Vector3.left;
                    Vector3 right = Vector3.right;

                    Vector3[] normales = new Vector3[]
                    {
	                // Left
	                left, left, left, left,
 
	                // Front
	                front, front, front, front,
 
	                // Back
	                back, back, back, back,
 
	                // Right
	                right, right, right, right,
 
	                // Top
	                up, up, up, up
                    };

                    Vector2 _00 = new Vector2(0f, 0f);
                    Vector2 _10 = new Vector2(1f, 0f);
                    Vector2 _01 = new Vector2(0f, 1f);
                    Vector2 _11 = new Vector2(1f, 1f);

                    Vector2[] uvs = new Vector2[]
                    {
 
	                // Left
	                _11, _01, _00, _10,
 
	                // Front
	                _11, _01, _00, _10,
 
	                // Back
	                _11, _01, _00, _10,
 
	                // Right
	                _11, _01, _00, _10,
 
	                // Top
	                _11, _01, _00, _10,
                    };

                    int[] triangles = new int[]
                    {
	                // Left
	                3 + 4 * 0, 1 + 4 * 0, 0 + 4 * 0,
                    3 + 4 * 0, 2 + 4 * 0, 1 + 4 * 0,
 
	                // Front
	                3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
                    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	                // Back
	                3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	                // Right
	                3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
                    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	                // Top
	                3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
                    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
                    };

                    Mesh tm = new Mesh {
                        vertices = vertices,
                        normals = normales,
                        triangles = triangles,
                        uv = uvs

                    }; ;
                    GameObject temp = new GameObject("temp");
                    temp.transform.parent = streetBuilding.transform;

                    temp.transform.rotation = streetBuilding.transform.rotation;
                    if (leftStreet) {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease / 2), 0, roadOffset);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, roadOffset);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, roadOffset);
                        }
                    }
                    else {
                        if (newGen && corner) {
                            temp.transform.localPosition = new Vector3(offset + (cornerOrEndDecrease), 0, (-roadOffset) - size);
                        }
                        else if (newGen) {
                            temp.transform.localPosition = new Vector3(offset + cornerOrEndDecrease, 0, (-roadOffset) - size);
                        }
                        else {
                            temp.transform.localPosition = new Vector3(offset, 0, (-roadOffset) - size);
                        }
                    }

                    offset += size + 1f;
                    MeshFilter m = temp.AddComponent<MeshFilter>();

                    m.mesh = tm;
                }
            }
        }

        //Can now detect if is a new generator or new generator has been build by this Generator
        protected override Vector2 RoadCrossing(Vector3 position) {
            List<Vector2> possibleCrossings = new List<Vector2>();
            if (curLenght >= 1) {
                //Check what quad you are in.
                foreach (Quad quad in settings.quads) {
                    if (position.x < quad.quadPosition.x && position.z < quad.quadPosition.y) {

                        //If quad is found, loop through all occupied possitions
                        foreach (Vector2 x in quad.occupiedHighway) {
                            //if there is a occupied possition found within range of our current position of X and Z
                            if (position.x < x.x + (settings.highwayClearance) && position.x > x.x - (settings.highwayClearance)) {
                                if (position.z < x.y + (settings.highwayClearance) && position.z > x.y - (settings.highwayClearance)) {
                                    DestroyGenerator();
                                    this.gameObject.SetActive(false);
                                }
                            }
                        }
                        foreach (Vector2 x in quad.occupied) {
                            //if there is a occupied possition found within range of our current position of X and Z
                            if (position.x < x.x + (laserDistance / roadCrossingSnapDistance) && position.x > x.x - (laserDistance / roadCrossingSnapDistance)) {
                                if (position.z < x.y + (laserDistance / roadCrossingSnapDistance) && position.z > x.y - (laserDistance / roadCrossingSnapDistance)) {
                                    possibleCrossings.Add(x);
                                }
                            }
                        }

                        if (possibleCrossings.Count >= 1) {
                            return ClosestPoint(possibleCrossings, position);
                        }
                    }
                }
            }
            return new Vector2(0, 0);
        }

        //Puts all generated meshes on to one mesh and puts them in the parent meshFilter
        void SingleMesh(GameObject building) {
            MeshFilter[] meshFilters = building.GetComponentsInChildren<MeshFilter>();

            Building b = building.AddComponent<Building>();
            b.meshRenderer.material = buildingMaterial;

            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            Matrix4x4 pTransform = building.transform.worldToLocalMatrix;
            int i = 0;
            while (i < meshFilters.Length) {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = pTransform * meshFilters[i].transform.localToWorldMatrix;
                meshFilters[i].gameObject.SetActive(false);
                i++;
            }
            b.meshFilter.mesh = new Mesh();
            b.meshFilter.mesh.CombineMeshes(combine, true, true);

            for (int x = meshFilters.Length - 1; x >= 0; x--) {
                Destroy(meshFilters[x].gameObject);
            }
            b.gameObject.SetActive(true);
            b.meshCollider.sharedMesh = b.meshFilter.mesh;
        }

        //Makes new StreetGenerators
        public void NewStreet() {
            Vector3 x = this.transform.eulerAngles;

            //Kijk naar +~90 graden
            float rot = Random.Range(settings.R_minAngle, settings.R_maxAngle);
            this.transform.Rotate(new Vector3(0, rot, 0));
            if (populationMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale < settings.R_minPopulation) {
                Vector2 p = RoadCrossing(laserPos.transform.position);
                if (p != new Vector2(0, 0)) {
                    return;
                }
                else {
                    Instantiate(BranchPrfab, this.transform.position, this.transform.rotation, null);
                    branchDistance = 0;
                }
            }
            this.transform.eulerAngles = x;

            //Kijk naar -~90 graden
            this.transform.Rotate(new Vector3(0, -rot, 0));
            if (populationMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale < settings.R_minPopulation) {
                Vector2 p = RoadCrossing(laserPos.transform.position);
                if (p != new Vector2(0, 0)) {
                    return;
                }
                else {
                    Instantiate(BranchPrfab, this.transform.position, this.transform.rotation, null);
                    branchDistance = 0;
                }            
            }
            this.transform.eulerAngles = x;
        }

        //Sets generator to be removed in the SettingsObject
        protected override void DestroyGenerator() {
            settings.removeRoads.Add(this.gameObject);
        }
    }
}
