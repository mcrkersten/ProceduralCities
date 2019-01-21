using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V02 {
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]

    public class Building : MonoBehaviour {
        public MeshFilter meshFilter;
        public MeshCollider meshCollider;
        public MeshRenderer meshRenderer;
        private Rigidbody rb;
        private int age;
        private float time = .1f;


        // Use this for initialization
        private void Awake() {
            age = SettingsObject.Instance.buildingAge++;
            meshFilter = this.gameObject.GetComponent<MeshFilter>();
            meshCollider = this.gameObject.GetComponent<MeshCollider>();
            rb = this.gameObject.AddComponent<Rigidbody>();
            meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            rb.isKinematic = true;
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
        }

        //If trigger by older building, destroy self
        private void OnTriggerEnter(Collider other) {
            if(other.gameObject.GetComponent<Building>() != null) {
                if (other.gameObject.GetComponent<Building>().age < age) {
                    Destroy(this.gameObject);
                }
            }                 
        }

        private void Update() {
            if(time < 0) {
                meshRenderer.enabled = true;
                Destroy(rb);
            }
            else {
                time = time - Time.deltaTime;
            }
        }
    }
}

