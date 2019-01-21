using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V02 {
    //Class is used for optimalization
    public class Quad : MonoBehaviour {      
        public Vector2 quadPosition;
        public List<Vector2> occupied = new List<Vector2>();
        public List<Vector2> occupiedHighway = new List<Vector2>();
    }
}