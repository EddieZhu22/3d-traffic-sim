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
        public Network network;
        public string[] names;
        public object[] data;
        public string[][] vals;
        public List<int> td_link_cumulative_departure = new List<int>(),
        td_link_out_flow_capacity = new List<int>(),
        td_link_cumulative_arrival = new List<int>();
        public List<float> td_link_waiting_time = new List<float>();
        public int link_seq_no,
        from_node_seq_no,
        to_node_seq_no,
        external_from_node,
        external_to_node,
        type,
        flow_volume,
        free_flow_travel_time_in_min,
        cumulative_arrival_count = 0,
        cumulative_departure_count = 0,
        cumulative_virtual_delay_count = 0;

        public float length, free_speed, VDF_alpha, VDF_beta, general_travel_time_in_min, general_travel_time_in_simu_interval, cost, lanes, link_capacity;

        public List<int> entrance_queue = new List<int>(), exit_queue = new List<int>();

        public void init()
        {
            // changes: Reuse VDF variable names to avoid redundancy.
            free_flow_travel_time_in_min = Convert.ToInt32(length / Mathf.Max(0.001f, free_speed) * 60);
            cost = free_flow_travel_time_in_min;
            for (int i = 0; i < Network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_MIN + 1; i++)
            {
                td_link_waiting_time.Add(0);
            }
            for (int i = 0; i < network.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL + 1; i++)
            {
                td_link_out_flow_capacity.Add(Convert.ToInt32(link_capacity / (60 * network.NUMBER_OF_SIMU_INTERVALS_PER_MIN)));
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
            cumulative_arrival_count = 0;
            cumulative_departure_count = 0;
            cumulative_virtual_delay_count = 0;
        }
        public void CalculateBPRFunction()
        {
            //Update Cost Of Link
            general_travel_time_in_min = free_flow_travel_time_in_min * (1 + VDF_alpha * (Mathf.Pow((flow_volume / Mathf.Max(0.00001f, link_capacity)), VDF_beta)));
            general_travel_time_in_simu_interval = general_travel_time_in_min * network.NUMBER_OF_SIMU_INTERVALS_PER_MIN;
            cost = general_travel_time_in_min;
        }
    }
}
