using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim
{
    public class LinkObject : Object //3D object representing 3D visualization
    {
        public Link link;
        private MeshRenderer mesh;
        public Vector3 fromPos, toPos;
        void Start()
        {
            float y = toPos.z-fromPos.z; // delta y
            float x = toPos.x-fromPos.x; // delta x
            float r = Mathf.Sqrt(Mathf.Pow(toPos.x-fromPos.x,2) + Mathf.Pow(toPos.z-fromPos.z,2)); // Calculate Total Distance Using Pythagorean Thereom

            transform.localScale = new Vector3(r,0.1f,1);
            Vector3 m = new Vector3((toPos.x+fromPos.x)/2,0,(toPos.z+fromPos.z)/2);
            transform.position = m;
            transform.eulerAngles = new Vector3(0,Mathf.Atan(y/x) * 180/Mathf.PI,0);
        }

        void Update()
        {

        }
    }

}
