using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// Building Class
/// A building object is generated
/// </summary>

namespace TrafficSim
{
    public class Building
    {
        [System.Serializable]
        public struct BuildingProperties
        {
            public List<Vector3> Points;
            public int poi_id;
            public string buildingType;
        }
        public string geometry;

        public BuildingProperties properties;
        public string[] names;
        public object[] data;

        public List<Vector3> ParseGeometry(string input)
        {
            Debug.Log(input); 
            List<Vector3> points = new List<Vector3>();
            //example string: POLYGON ((-111.932196 33.415302, -111.932196 33.4150871, -111.9314922 33.4150871, -111.9314922 33.415302, -111.932196 33.415302)
            string[] strArr = input.Split(new char[] {',',' '});
            for (int i = 0; i < strArr.Length; i++)
            {
                strArr[i] = Regex.Replace(strArr[i], "[^0-9.]", "");

                if((i) % 2 == 0 && i != 0)
                {
                    points.Add(new Vector3(float.Parse(strArr[i - 1]),0,float.Parse(strArr[i])));
                } 
            }
            return points;
        } 
    }
}

