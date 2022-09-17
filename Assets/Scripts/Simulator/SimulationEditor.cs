using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using PathCreation;

namespace TrafficSim
{
    public class SimulationEditor : MonoBehaviour
    {
        [Header("File Names")]
        [SerializeField] private string Node_File_Name;
        [SerializeField] private string Link_File_Name, Agent_File_Name;

        [Header("Attributes")]
        public Network network;
        public float PosMultiplier;
        [SerializeField] private CSVReader reader;
        [SerializeField] private Vector3 offset;

        [Header("Objects")]
        [SerializeField] private GameObject Node_Object;
        [SerializeField] private GameObject Link_Object, Agent_Object;

        [Header("Lists, Arrays, Dictionaries")]
        public List<GameObject> Link_Obj_List;
        public List<LinkObject> Link_Comp_List;
        public List<GameObject> Node_Obj_List;
        private int[,] from_to_node_pairs;
        void Start()
        {
            ReadCSV();
        }
        private void ReadCSV() // Reads the CSV file
        {
            reader = new CSVReader();
            reader.CreateNodeCSV(network, Node_File_Name);
            reader.CreateLinkCSV(network, Link_File_Name);
            reader.CreateAgentCSV(network, Agent_File_Name);
        }

        public void RunSimulation() //Run the Simulation Process
        {
            Stopwatch stopwatch = new Stopwatch();
            //simManager = new SimulationManager();
            network.init();
            network.allocate();
            stopwatch.Start();

            network.g_find_shortest_path_for_agent();
            network.g_TrafficSimulation();
            stopwatch.Stop();
            print("simulation processing time: {0: .2f}" + (stopwatch.Elapsed) + "s");
        }

        public void CreateNetworkObjects() // Instantiate All 3D Network Objects
        {
            // Loop Through Nodes and instantiate the GameObject
            for (int i = 0; i < network.node_list.Count; i++)
            {
                GameObject node = Instantiate(Node_Object);
                SetNodeAttributes(node,i);
                NodeObject nodeComponent = node.GetComponent<NodeObject>();
                nodeComponent.node = network.node_list[i];
                Node_Obj_List.Add(node);
            }

            //Set Length of 2D Array
            from_to_node_pairs = new int[network.node_list.Count, network.node_list.Count];

            // Loop Through Links and instantiate the GameObject
            for (int i = 0; i < network.link_list.Count; i++)
            {
                GameObject link = Instantiate(Link_Object);
                LinkObject linkComponent = link.GetComponent<LinkObject>();
                SetLinkAttributes(linkComponent, link, i, false);
                
                Link_Obj_List.Add(link);
                Link_Comp_List.Add(linkComponent);
            }

        }

        //Calculate Offset
        private Vector3 Offset(Transform objT, Vector3 vector3)
        {
            if (objT.rotation.y != 0)
                return vector3;
            else
                return new Vector3(vector3.z, vector3.y, vector3.x);
        }
        public void SetNodeAttributes(GameObject node, int i)
        {
            node.transform.position = network.external_node_pos[i] / PosMultiplier;
        }

        public void SetLinkAttributes(LinkObject linkComponent, GameObject link, int i, bool external)
        {
            int from_node_seq_no = network.link_list[i].properties.from_node_seq_no;
            int to_node_seq_no = network.link_list[i].properties.to_node_seq_no;

            linkComponent.link = network.link_list[i];
            linkComponent.fromPos = network.external_node_pos[from_node_seq_no] / PosMultiplier;
            linkComponent.toPos = network.external_node_pos[to_node_seq_no] / PosMultiplier;

            from_to_node_pairs[to_node_seq_no, from_node_seq_no] = 10;

            //Set a Position Without Offset
            linkComponent.SetPosition();

            //check if method is being called from within the simulation editor as part of instantiation.
            if(external == false)
            {
                //Find the Offset Based On Whether the same Link has From To Pair is the same (ex. [from: 25, to: 15], [to: 15, from: 25])
                if (from_to_node_pairs[to_node_seq_no, from_node_seq_no] == from_to_node_pairs[from_node_seq_no, to_node_seq_no])
                {
                    linkComponent.fromPos += Offset(link.transform, offset);
                    linkComponent.toPos += Offset(link.transform, offset);
                    linkComponent.offsetVal = Offset(link.transform, offset);
                }
                else
                {
                    linkComponent.fromPos -= Offset(link.transform, offset);
                    linkComponent.toPos -= Offset(link.transform, offset);
                    linkComponent.offsetVal = -Offset(link.transform, offset);
                }
            }
            linkComponent.fromPos += linkComponent.offsetVal;
            linkComponent.toPos += linkComponent.offsetVal;

            //Set the new Position
            linkComponent.SetPosition();
        }
    }
}

