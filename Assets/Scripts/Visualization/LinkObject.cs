using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficSim
{
    public class LinkObject : Object //3D object representing 3D visualization
    {
        public Link link;

        // public property shows in Inspector
        public Link.LinkProperties properties;

        public Vector3 fromPos, toPos, offsetVal;
        void Start()
        {
            SetProperties();
        }

        void Update()
        {

        }
        public void SetPosition()
        {
            float y = toPos.z - fromPos.z; // delta y
            float x = toPos.x - fromPos.x; // delta x
            float r = Mathf.Sqrt(Mathf.Pow(toPos.x - fromPos.x, 2) + Mathf.Pow(toPos.z - fromPos.z, 2)); // Calculate Total Distance Using Pythagorean Thereom

            transform.localScale = new Vector3(r, 0.1f, .5f);
            Vector3 m = new Vector3((toPos.x + fromPos.x) / 2, 0, (toPos.z + fromPos.z) / 2);
            transform.position = m;
            transform.eulerAngles = new Vector3(0, - (Mathf.Atan(y / x) * 180 / Mathf.PI), 0);
        }  
        public void SetProperties()
        {
            properties = link.properties;
        }
    }

}
