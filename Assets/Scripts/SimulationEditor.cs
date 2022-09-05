using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;

namespace TrafficSim
{
    public class SimulationEditor : MonoBehaviour
    {
        [SerializeField] private string Node_File_Name, Link_File_Name, Agent_File_Name;
        public CSVReader reader;
        public Network network;

        public GameObject Node_Object, Link_Object, Agent_Object;
        void Start()
        {     
            Stopwatch stopwatch = new Stopwatch();
            reader = new CSVReader();
            //simManager = new SimulationManager();
            CreateObjects();
            network.init();
            network.allocate();
            stopwatch.Start();

            network.g_find_shortest_path_for_agent();
            network.g_TrafficSimulation();
            stopwatch.Stop();
            print("simulation processing time: {0: .2f}" + (stopwatch.Elapsed) + "s");
        }

        void Update()
        {
        }
        private void CreateObjects()
        {
            reader.CreateNodeCSV(network, Node_File_Name);
            reader.CreateLinkCSV(network, Link_File_Name);
            reader.CreateAgentCSV(network, Agent_File_Name);
        }
    }
}

