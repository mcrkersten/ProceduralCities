using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace v01 {
    public class HighwayGeneratorV01 : MonoBehaviour {

        //private SettingsObject settings;

        //[Header("Highway Branch object")]
        //public GameObject BranchPrfab;
        //public bool canBranch;
        //private int branchDistance = 0;

        //private Transform approvedPossition;
        //private bool waterMapCheckResult;
        //private GameObject laserPos;

        //private int angle;
        //private int rays;
        //private float laserDistance;


        ////Maps Higher position = Stronger
        //private Texture2D waterMap;
        //private Texture2D populationMap;
        ////TO-DO: HeightMap

        //[Header("Debug visualization")]
        //public LineRenderer lr;

        //[SerializeField]
        //Vector3 bestPosVector;
        //Vector3 bestRotVector;


        //// Use this for initialization
        //void Awake() {
        //    settings = SettingsObject.Instance;
        //    angle = settings.H_angle;
        //    rays = settings.rays;
        //    laserDistance = settings.rays;

        //    populationMap = settings.populationMap;
        //    waterMap = settings.waterMap;

        //    //Line Renderer
        //    lr = this.GetComponent<LineRenderer>();
        //    lr.positionCount = 2;
        //    lr.SetPosition(0, this.transform.position);
        //    lr.SetPosition(1, laserPos.transform.position);
        //}


        //public void InitBranch(Vector3 rot, Vector3 pos) {
        //    float t = Random.Range(0 - settings.H_branchAngle, 0 + settings.H_branchAngle);
        //    this.transform.position = new Vector3(pos.x, pos.y, pos.z);
        //    this.transform.eulerAngles = new Vector3(rot.x, rot.y + t, rot.z);
        //    BuildFreeway();
        //}

        //[ContextMenu("Build Freeway")]
        //void BuildFreeway() {
        //    GetBestPosition();

        //    if (canBranch) {
        //        BranchHighway();
        //    }
        //    DrawNewRoad();      //If best position is found
        //    NewTestPosition();  //Restart system for next Segtment.

        //    //if within populationmap size, restart this function
        //    if (this.transform.position.x > 0 && this.transform.position.x < settings.populationMap.width && this.transform.position.z > 0 && this.transform.position.z < settings.populationMap.height) {
        //        BuildFreeway();
        //    }
        //}


        //void GetBestPosition() {
        //    //New Position Check
        //    approvedPossition = null;
        //    waterMapCheckResult = true;

        //    if (settings.followPopulationMap) {
        //        approvedPossition = PopulationMapCheck();

        //        if (settings.followWaterMap) {
        //            waterMapCheckResult = WaterMapCheck(approvedPossition);
        //        }
        //        HighwayRoadFinalCheck(waterMapCheckResult);
        //    }
        //}


        //void HighwayRoadFinalCheck(bool waterCheck) {
        //    if (waterCheck) {
        //        bestPosVector = approvedPossition.position;
        //        bestRotVector = approvedPossition.eulerAngles;
        //    }
        //    else if (settings.followWaterBody) {
        //        FollowWaterbody();
        //    }
        //    else {
        //        //Bridge?
        //        //Road-End?
        //        BuildFreeway();
        //    }
        //}

        ////Highway branches new highway
        //void BranchHighway() {
        //    branchDistance++;
        //    if (branchDistance < settings.H_minimalBranchDistance) {
        //        int spawnNumber = Random.Range(0, 100 + 1);
        //        if (spawnNumber < settings.H_branchProbability) {
        //            GameObject branch = Instantiate(BranchPrfab, null);
        //            branch.GetComponent<HighwayGeneratorV01>().InitBranch(this.transform.eulerAngles, this.transform.position);
        //            branchDistance = 0;
        //        }
        //    }
        //}

        ////Draws new road if approved
        //void DrawNewRoad() {
        //    GameObject newLine = new GameObject("Highway");

        //    //Line renderer
        //    newLine.transform.position = this.transform.position;
        //    LineRenderer nlr = newLine.AddComponent<LineRenderer>();
        //    nlr.positionCount = 2;
        //    nlr.SetPosition(0, this.transform.position);
        //    nlr.SetPosition(1, bestPosVector);
        //    nlr.startWidth = 10;
        //    nlr.endWidth = 10;

        //}

        ////Setup new position after highway segment is build
        //void NewTestPosition() {
        //    this.transform.eulerAngles = bestRotVector;
        //    this.transform.position = bestPosVector;
        //    lr.SetPosition(0, this.transform.position);
        //    lr.SetPosition(1, laserPos.transform.position);
        //}

        ////the road follows the body of water
        //void FollowWaterbody() {
        //    for (int i = 0; i < rays; i++) {

        //    }
        //}


        //Transform PopulationMapCheck() {
        //    for (int i = 0; i < rays; i++) {
        //        int pos = Mathf.RoundToInt(Random.Range(this.transform.eulerAngles.y - angle / 2, this.transform.eulerAngles.y + angle / 2));
        //        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, pos, this.transform.eulerAngles.z);
        //        lr.SetPosition(1, laserPos.transform.position);

        //        //PopulationMap Check
        //        //Population Map Constraints
        //        float heighest = 1;
        //        float posPopulationMap = populationMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale;

        //        //Best Pos on PopulationMap
        //        if (posPopulationMap < heighest) {
        //            heighest = posPopulationMap;
        //            approvedPossition = laserPos.transform;
        //        }
        //    }
        //    return approvedPossition;
        //}


        //bool WaterMapCheck(Transform pos) {
        //    float posWaterMap = waterMap.GetPixel(Mathf.RoundToInt(laserPos.transform.position.x), Mathf.RoundToInt(laserPos.transform.position.z)).grayscale;
        //    if (posWaterMap > .5) {
        //        return true;
        //    }
        //    else {
        //        return false;
        //    }
        //}
    }
}
