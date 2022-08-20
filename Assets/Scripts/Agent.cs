using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace TrafficSim
{
    public class Agent : MonoBehaviour
    {
        /*
        self.agent_id = agent_id
            self.departure_time_in_simu_interval = int(self.departure_time_in_min*60/NUMBER_OF_SECONDS_PER_SIMU_INTERVAL + 0.5)
            self.feasible_path_exist_flag = False
            self.path_fixed_flag=path_fixed_flag
        */
        public SimulationManager manager;
        public string[] names;
        public object[] data;
        public List<int> path_node_seq;
        public List<int> path_node_seq_no_list;
        public List<int> path_link_seq_no_list;
        public int agent_seq_no = 0;
        public int agent_id;
        public string agent_type;
        public double departure_time_in_min;
        public int departure_time_in_simu_interval;
        public int current_link_seq_no_in_path;
        public int o_node_id,
        d_node_id,
        PCE, path_fixed_flag;
        public double path_cost;
        public bool b_generated, b_complete_trip;
        void Start()
        {
            departure_time_in_simu_interval = Convert.ToInt32(departure_time_in_min * 60 / SimulationManager.NUMBER_OF_SECONDS_PER_SIMU_INTERVAL + 0.5);
        }

        // Update is called once per frame
        void Update()
        {

        }
        void Initialize_for_simulation()
        {

        }
    }
}
