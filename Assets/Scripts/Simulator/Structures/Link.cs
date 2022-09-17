using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


/// <summary>
/// Link Class
/// A link object is generated
/// </summary>
namespace TrafficSim
{
    public class Link
    {
        [Serializable]
        public struct LinkProperties
        {
            public int link_seq_no,from_node_seq_no,to_node_seq_no,external_from_node,external_to_node,type,flow_volume,free_flow_travel_time_in_min,cumulative_arrival_count,cumulative_departure_count,cumulative_virtual_delay_count;
            public float length, free_speed, VDF_alpha, VDF_beta, general_travel_time_in_min, general_travel_time_in_simu_interval, cost, lanes, link_capacity;
        }

        public LinkProperties properties;
        public Network network;
        public string[] names;
        public object[] data;
        public string[][] vals;

        // Lists
        public List<int> td_link_cumulative_departure = new List<int>(),
        td_link_out_flow_capacity = new List<int>(),
        td_link_cumulative_arrival = new List<int>();
        public List<float> td_link_waiting_time = new List<float>();
        public List<int> entrance_queue = new List<int>(), exit_queue = new List<int>();

        public void init()
        {
            // changes: Reuse VDF variable names to avoid redundancy.
            properties.free_flow_travel_time_in_min = Convert.ToInt32(properties.length / Mathf.Max(0.001f, properties.free_speed) * 60);
            properties.cost = properties.free_flow_travel_time_in_min;
            for (int i = 0; i < Network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_MIN + 1; i++)
            {
                td_link_waiting_time.Add(0);
            }
            for (int i = 0; i < network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL + 1; i++)
            {
                td_link_out_flow_capacity.Add(Convert.ToInt32(properties.link_capacity / (60 * network.NUMBER_OF_SIMU_INTERVALS_PER_MIN)));
            }
            for (int i = 0; i < network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL + 1; i++)
            {
                td_link_cumulative_arrival.Add(0);
            }
            for (int i = 0; i < network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL + 1; i++)
            {
                td_link_cumulative_departure.Add(0);
            }

        }
        public void ResetMOE()
        {
            //Reset Measures Of Effectiveness
            properties.cumulative_arrival_count = 0;
            properties.cumulative_departure_count = 0;
            properties.cumulative_virtual_delay_count = 0;
        }
        public void CalculateBPRFunction()
        {
            //Update Cost Of Link
            properties.general_travel_time_in_min = properties.free_flow_travel_time_in_min * (1 + properties.VDF_alpha * (Mathf.Pow((properties.flow_volume / Mathf.Max(0.00001f, properties.link_capacity)), properties.VDF_beta)));
            properties.general_travel_time_in_simu_interval = properties.general_travel_time_in_min * network.NUMBER_OF_SIMU_INTERVALS_PER_MIN;
            properties.cost = properties.general_travel_time_in_min;
        }
    }
}
