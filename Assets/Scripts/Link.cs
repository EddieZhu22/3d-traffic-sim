using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


/// <summary>
/// Link Class
/// A link object is generated
/// </summary>
public class Link : MonoBehaviour
{
    public SimulationManager manager;
    public string[] names;
    public object[] data;
    public string[][] vals;
    public int link_seq_no, 
    from_node_seq_no, 
    to_node_seq_no, 
    external_from_node, 
    external_to_node, 
    type, 
    flow_volume, 
    free_flow_travel_time_in_min, 
    td_link_out_flow_capacity, 
    td_link_cumulative_arrival, 
    td_link_cumulative_departure, 
    cumulative_arrival_count = 0,
    cumulative_departure_count = 0,
    cumulative_virtual_delay_count = 0,
    td_link_waiting_time;

    public float length, free_speed, VDF_alpha, VDF_beta, general_travel_time_in_min, general_travel_time_in_simu_interval, cost, lanes, link_capacity;

    public List<Agent> entrance_queue, exit_queue;

    private void Start() 
    {
        init();
    }
    private void init()
    {
        // changes: Reuse VDF variable names to avoid redundancy.
        free_flow_travel_time_in_min = Convert.ToInt32(length / Mathf.Max(0.001f, free_speed) * 60);
        cost = free_flow_travel_time_in_min;
        td_link_out_flow_capacity = Convert.ToInt32(link_capacity/(60 * manager.NUMBER_OF_SIMU_INTERVALS_PER_MIN)) * (manager.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL + 1);
        td_link_cumulative_arrival = manager.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL+1;
        td_link_cumulative_departure = manager.LENGTH_OF_SIMULATION_TIME_HORIZON_IN_INTERVAL+1;

    }
    private void ResetMOE()
    {
        cumulative_arrival_count = 0;
        cumulative_departure_count = 0;
        cumulative_virtual_delay_count = 0;
    }
    private void CalculateBPRFunction()
    {
        general_travel_time_in_min = free_flow_travel_time_in_min*(1 + VDF_alpha*(Mathf.Pow((flow_volume / Mathf.Max(0.00001f, link_capacity)),VDF_beta)));
        general_travel_time_in_simu_interval = general_travel_time_in_min * manager.NUMBER_OF_SIMU_INTERVALS_PER_MIN;
        cost = general_travel_time_in_min;
    }
}
