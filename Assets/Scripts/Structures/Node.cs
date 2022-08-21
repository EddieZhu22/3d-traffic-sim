using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node Class
/// A node object is generated
/// </summary>

namespace TrafficSim
{
    public class Node
    {
        public string[] names;
        public object[] data;
        public string[][] vals;

        public List<Link> outgoing_link_list;
        public List<Link> incoming_link_list;

        public int node_seq_no, external_node_id, zone_id;
        public float xcoord, ycoord;

    }
}

