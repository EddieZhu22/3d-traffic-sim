using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node Class
/// A node object is generated
/// </summary>

namespace TrafficSim
{
    [System.Serializable]
    public class Node
    {
        [HideInInspector]
        public string[] names;
        public object[] data;
        public List<Link> outgoing_link_list = new List<Link>();
        public List<Link> incoming_link_list = new List<Link>();

        public int node_seq_no, external_node_id, zone_id;
        public float xcoord, ycoord;

    }
}

