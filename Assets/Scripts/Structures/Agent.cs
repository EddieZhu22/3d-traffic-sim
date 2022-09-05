using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace TrafficSim
{
    public class Agent
    {
        /*
        self.agent_id = agent_id
            self.departure_time_in_simu_interval = int(self.departure_time_in_min*60/NUMBER_OF_SECONDS_PER_SIMU_INTERVAL + 0.5)
        */
        public Network network;
        public string[] names;
        public object[] data;
        public List<int> path_node_seq = new List<int>();
        public List<int> path_node_seq_no_list = new List<int>();
        public List<int> path_link_seq_no_list = new List<int>();
        public int agent_seq_no = 0;
        public int agent_id;
        public string agent_type;
        public float departure_time_in_min;
        public int departure_time_in_simu_interval;
        public int current_link_seq_no_in_path;
        public int o_node_id,
        d_node_id,
        PCE, path_fixed_flag;
        public double path_cost;
        public bool b_generated, b_complete_trip, feasible_path_exist_flag;
        public List<float> veh_link_departure_time_in_simu_interval = new List<float>(), veh_link_arrival_time_in_simu_interval = new List<float>();

        // Update is called once per frame
        void Update()
        {

        }
        public void Initialize_for_simulation()
        {
            if (path_node_seq_no_list.Count > 0)
            {
                for (int i = 0; i < path_node_seq_no_list.Count; i++)
                    veh_link_arrival_time_in_simu_interval.Add(-1);
                for (int i = 0; i < path_node_seq_no_list.Count; i++)
                    veh_link_departure_time_in_simu_interval.Add(-1);
                veh_link_arrival_time_in_simu_interval[0] = departure_time_in_simu_interval;
            }
        }
    }
}
