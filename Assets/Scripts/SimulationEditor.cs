using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            reader = new CSVReader();
            //simManager = new SimulationManager();
            CreateObjects();
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

