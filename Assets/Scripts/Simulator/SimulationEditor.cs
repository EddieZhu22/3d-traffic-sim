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
        [SerializeField] private CSVReader reader;
        [SerializeField] private Network network;
        [SerializeField] private int PosMultiplier;

        [Header("Objects")]
        [SerializeField] private GameObject Node_Object; 
        [SerializeField] private GameObject Link_Object, Agent_Object;
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
        public void RunSimulation()
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
        public void CreateNetworkObjects()
        {
            for (int i = 0; i < network.node_list.Count; i++)
            {
                GameObject node = Instantiate(Node_Object);
                node.transform.position = network.external_node_pos[i] / PosMultiplier;

                NodeObject nodeComponent = node.GetComponent<NodeObject>();
                nodeComponent.node = network.node_list[i];
            }
            for (int i = 0; i < network.link_list.Count; i++)
            {
                GameObject link = Instantiate(Link_Object);
                LinkObject linkComponent = link.GetComponent<LinkObject>();

                linkComponent.link = network.link_list[i];
                linkComponent.fromPos = network.external_node_pos[network.link_list[i].from_node_seq_no] / PosMultiplier;
                linkComponent.toPos = network.external_node_pos[network.link_list[i].to_node_seq_no] / PosMultiplier;

            }

        }
        public void GenerateNetworkPath()
        {

        }
    }
}

