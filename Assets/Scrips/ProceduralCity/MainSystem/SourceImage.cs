using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V02 {
    //Used for debug
    public class SourceImage : MonoBehaviour {

        private Texture2D image;
        private int xValue;
        private int zValue;
        private Material material;

        private Mesh meshPlane;
        private Renderer rd;
        private MeshFilter meshFilter;

        // Use this for initialization
        void Start() {
            image = SettingsObject.Instance.populationMap;
            rd = GetComponent<Renderer>();
            meshFilter = GetComponent<MeshFilter>();
            GeneratePlane();
        }

        void GeneratePlane() {
            meshPlane = new Mesh();

            Vector3[] newVertices = new Vector3[4];
            newVertices[0] = new Vector3(0, 0, 0);                           //Cord Left down
            newVertices[1] = new Vector3(image.height, 0, 0);                //Cord right down

            newVertices[2] = new Vector3(0, 0, image.width);                 //Cord Left up
            newVertices[3] = new Vector3(image.height, 0, image.width);      //Cord Left down

            int[] tri = new int[6];                                          //Tri cordinates
            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            Vector3[] normals = new Vector3[4];                              //Normals direction
            normals[0] = Vector3.up;
            normals[1] = Vector3.up;
            normals[2] = Vector3.up;
            normals[3] = Vector3.up;

            Vector2[] uv = new Vector2[4];                                   //Mesh UV
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(1, 0);
            uv[2] = new Vector2(0, 1);
            uv[3] = new Vector2(1, 1);

            meshPlane.vertices = newVertices;
            meshPlane.triangles = tri;
            meshPlane.normals = normals;
            meshPlane.uv = uv;

            meshFilter.mesh = meshPlane;

            material = new Material(Shader.Find("Diffuse")) { mainTexture = image };
            rd.material = material;
        }
    }
}
