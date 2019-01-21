using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSystem : MonoBehaviour {

    [Header("Floor Settings")]
    public int xMeshSize;
    public int zMeshSize;
    public Material materialGround;

    [Header("Grid Settings")]
    public int gridLines;
    public float lineThinkness;
    public Material materialGrid;

    private static GroundSystem instance = null;
    public static GroundSystem Instance
    {
        get {
            if (instance == null) {
                // This is where the magic happens.
                //  FindObjectOfType(...) returns the first PlayerController object in the scene.
                instance = FindObjectOfType(typeof(GroundSystem)) as GroundSystem;
            }

            // If it is still null, create a new instance
            if (instance == null) {
                GameObject obj = new GameObject("PlayerController");
                instance = obj.AddComponent(typeof(GroundSystem)) as GroundSystem;
                Debug.Log("Could not locate an StreetGenerator object.  PlayerController was Generated Automaticly.");
            }
            return instance;
        }
    }

    Mesh mesh;
    Renderer rd;
    MeshFilter mf;



    void Start() {
        GenerateFloor();
        GenerateGrid();
    }

    void GenerateFloor() {

        mesh = new Mesh();
        rd = GetComponent<Renderer>();
        mf = GetComponent<MeshFilter>();

        Vector3[] newVertices = new Vector3[4];
        newVertices[0] = new Vector3((-xMeshSize/2) - .5f, 0, (-zMeshSize/2) - .5f);
        newVertices[1] = new Vector3((xMeshSize/2) + .5f, 0, (-zMeshSize/2) - .5f);

        newVertices[2] = new Vector3((-xMeshSize/2) - .5f, 0, (zMeshSize/2) + .5f);
        newVertices[3] = new Vector3((xMeshSize/2) + .5f, 0, (zMeshSize/2) + .5f);

        int[] tri = new int[6];
        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        tri[3] = 2;
        tri[4] = 3;
        tri[5] = 1;

        Vector3[] normals = new Vector3[4];
        normals[0] = -Vector3.up;
        normals[1] = -Vector3.up;
        normals[2] = -Vector3.up;
        normals[3] = -Vector3.up;

        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        mesh.vertices = newVertices;
        mesh.triangles = tri;
        mesh.normals = normals;
        mesh.uv = uv;

        mf.mesh = mesh;
        rd.material = materialGround;
    }

    void GenerateGrid() {

        for (int x = 0; x <= gridLines; x++) {
            mesh = new Mesh();
            Vector3[] newVertices = new Vector3[4];
            int[] tri = new int[6];
            Vector3[] normals = new Vector3[4];
            Vector2[] uv = new Vector2[4];

            GameObject grid = new GameObject("GridLine X" + x);
            grid.transform.parent = this.transform;
            grid.transform.position = new Vector3((-xMeshSize / 2) + x, 0, (-zMeshSize / 2));

            Renderer gridR = grid.AddComponent<MeshRenderer>();
            MeshFilter gridMF = grid.AddComponent<MeshFilter>();

            newVertices[0] = new Vector3(-.5f, .01f, -.5f);
            newVertices[1] = new Vector3(-.5f, .01f, 100.5f);
            newVertices[2] = new Vector3(lineThinkness-.5f, .01f, -.5f);
            newVertices[3] = new Vector3(lineThinkness-.5f, .01f, 100.5f);

            tri[0] = 1;
            tri[1] = 2;
            tri[2] = 0;

            tri[3] = 1;
            tri[4] = 3;
            tri[5] = 2;

            normals[0] = Vector3.up;
            normals[1] = Vector3.up;
            normals[2] = Vector3.up;
            normals[3] = Vector3.up;

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.vertices = newVertices;
            mesh.triangles = tri;
            mesh.normals = normals;
            mesh.uv = uv;

            gridMF.mesh = mesh;
            gridR.material = materialGrid;
        }

        for (int z = 0; z <= gridLines; z++) {
            mesh = new Mesh();
            Vector3[] newVertices = new Vector3[4];
            int[] tri = new int[6];
            Vector3[] normals = new Vector3[4];
            Vector2[] uv = new Vector2[4];

            GameObject grid = new GameObject("GridLine Z" + z);
            grid.transform.position = new Vector3((-xMeshSize / 2), 0, (-zMeshSize / 2) + z);
            grid.transform.parent = this.transform;

            Renderer gridR = grid.AddComponent<MeshRenderer>();
            MeshFilter gridMF = grid.AddComponent<MeshFilter>();

            newVertices[0] = new Vector3(-.5f, .01f, -.5f);
            newVertices[1] = new Vector3(100.5f, .01f, -.5f);
            newVertices[2] = new Vector3(-.5f, .01f, lineThinkness-.5f);
            newVertices[3] = new Vector3(100.5f, .01f, lineThinkness -.5f);

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            normals[0] = Vector3.up;
            normals[1] = Vector3.up;
            normals[2] = Vector3.up;
            normals[3] = Vector3.up;

            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            mesh.vertices = newVertices;
            mesh.triangles = tri;
            mesh.normals = normals;
            mesh.uv = uv;

            gridMF.mesh = mesh;
            gridR.material = materialGrid;
        }

    }
}
